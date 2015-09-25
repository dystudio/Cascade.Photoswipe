var initPhotoSwipeFromDOM = function (gallerySelector, options) {

    // parse slide data (url, title, size ...) from DOM elements 
    // (children of gallerySelector)
    var parseThumbnailElements = function (el) {
        var thumbElements = el.childNodes,
            numNodes = thumbElements.length,
            items = [],
            figureEl,
            linkEl,
            size,
            title,
            limit,
            item;

        for (var i = 0; i < numNodes; i++) {

            figureEl = thumbElements[i]; // <figure> element

            // include only element nodes 
            if (figureEl.nodeType !== 1) {
                continue;
            }
            if (!isSelected(options.selectedClass, figureEl)) {
                continue;
            }

            linkEl = figureEl.children[0]; // <a> element

            size = linkEl.getAttribute('data-size').split('x');
            limit = linkEl.getAttribute('data-limit').split('x');
            title = linkEl.getAttribute('data-title');


            // create slide object
            item = {
                src: linkEl.getAttribute('href'),
                sizew: parseInt(size[0], 10),
                sizeh: parseInt(size[1], 10),
                limitw: parseInt(limit[0], 10),
                limith: parseInt(limit[1], 10),
                html: linkEl.getAttribute('data-html'),
                title: title
            };


            //if (figureEl.children.length > 1) {
            //    // <figcaption> content
            //    item.title = figureEl.children[1].innerHTML;
            //}

            if (linkEl.children.length > 0) {
                // <img> thumbnail element, retrieving thumbnail url
                item.msrc = linkEl.children[0].getAttribute('src');
            }

            item.el = figureEl; // save link to element for getThumbBoundsFn
            items.push(item);
        }

        return items;
    };

    var isSelected = function (selectedClass, figureEl) {
        if (selectedClass) {
            // include only elements matching specified class (if any)
            var classes = figureEl.getAttribute("class");
            var arClasses = classes.split(' ');
            var select = false;
            for (var n = 0; n < arClasses.length; ++n) {
                if (arClasses[n] == selectedClass) {
                    return true;
                }
            }
            return false;
        }
        return true;
    }
    // find nearest parent element
    var closest = function closest(el, fn) {
        return el && (fn(el) ? el : closest(el.parentNode, fn));
    };

    // triggers when user clicks on thumbnail
    var onThumbnailsClick = function (e) {
        e = e || window.event;
        e.preventDefault ? e.preventDefault() : e.returnValue = false;

        var eTarget = e.target || e.srcElement;

        // find root element of slide
        var clickedListItem = closest(eTarget, function (el) {
            return (el.tagName && el.tagName.toUpperCase() === 'FIGURE');
        });

        if (!clickedListItem) {
            return;
        }

        // find index of clicked item by looping through all child nodes
        // alternatively, you may define index via data- attribute
        var clickedGallery = clickedListItem.parentNode,
            childNodes = clickedListItem.parentNode.childNodes,
            numChildNodes = childNodes.length,
            nodeIndex = 0,
            index;

        for (var i = 0; i < numChildNodes; i++) {
            if (childNodes[i].nodeType !== 1) {
                continue;
            }
            if (!isSelected(options.selectedClass, childNodes[i])) {
                continue;
            }

            if (childNodes[i] === clickedListItem) {
                index = nodeIndex;
                break;
            }
            nodeIndex++;
        }



        if (index >= 0) {
            // open PhotoSwipe if valid index found
            openPhotoSwipe(index, clickedGallery);
        }
        return false;
    };

    // parse picture index and gallery index from URL (#&pid=1&gid=2)
    var photoswipeParseHash = function () {
        var hash = window.location.hash.substring(1),
        params = {};

        if (hash.length < 5) {
            return params;
        }

        var vars = hash.split('&');
        for (var i = 0; i < vars.length; i++) {
            if (!vars[i]) {
                continue;
            }
            var pair = vars[i].split('=');
            if (pair.length < 2) {
                continue;
            }
            params[pair[0]] = pair[1];
        }

        if (params.gid) {
            params.gid = parseInt(params.gid, 10);
        }

        if (!params.hasOwnProperty('pid')) {
            return params;
        }
        params.pid = parseInt(params.pid, 10);
        return params;
    };



    var openPhotoSwipe = function (index, galleryElement, disableAnimation) {
        var pswpElement = document.querySelectorAll('.pswp')[0],
            gallery,
            items;

        items = parseThumbnailElements(galleryElement);

        // define options (if needed)
        options.index = index;
        // define gallery index (for URL)
        options.galleryUID = galleryElement.getAttribute('data-pswp-uid'),

            options.getThumbBoundsFn = function (index) {
                // See Options -> getThumbBoundsFn section of documentation for more info
                var thumbnail = items[index].el.getElementsByTagName('img')[0], // find thumbnail
                    pageYScroll = window.pageYOffset || document.documentElement.scrollTop,
                    rect = thumbnail.getBoundingClientRect();

                return { x: rect.left, y: rect.top + pageYScroll, w: rect.width };
            };

        if (disableAnimation) {
            options.showAnimationDuration = 0;
        }

        // Pass data to PhotoSwipe and initialize it
        gallery = new PhotoSwipe(pswpElement, PhotoSwipeUI_Cascade, items, options);

        if (options.phone) {
            gallery.listen('close', function () { window.history.back(); });
        }

        ///////////////////////////////////////////////
        // FlipCard support (DAG) added for Clayton
        //
        // There are 3 items in the DOM. All 3 are flipped because PS doesn't identify which one of 
        // the three is currently displayed. I know it is always the middle one, so I suppose I could
        // construct a slector for that, but flipping all 3 doesn't seem to be a problem.

        if (options.flip) {
          
            // unflip before prev/next: beforeChange is emitted AFTER the currItem has been updated, so use indexDiff 
            // shortcomings with this approach include:
            // 1. item is flipped back but the transition to the next/previous slide overlaps
            // 2. beforeChange isn't triggered until well into a swipe, meaning the backside of the next/prev card is shown
            // still searching for a better way that doesn't involve hacking photoswipe.js
            gallery.listen('beforeChange', function (indexDiff) {
                if (indexDiff) {
                    var item = gallery.items[gallery.getCurrentIndex() - indexDiff];
                    if (item.flipped) {
                        $('.card-container').flip();
                        item.flipped = false;
                    }
                }
            });
        }




        ////////////////////////////////////////////////////////////////// Caption positioning ///////////////////////////
        //
        // Pending stackoverflow question on how to identify image element to measure position of.
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // handy function for getting relative positions
        function getOffset(el) {
            var _x = 0;
            var _y = 0;
            while (el && !isNaN(el.offsetLeft) && !isNaN(el.offsetTop)) {
                _x += el.offsetLeft - el.scrollLeft;
                _y += el.offsetTop - el.scrollTop;
                el = el.offsetParent;
            }
            return { top: _y, left: _x };
        }
        function getOffsetRect(elem) {
            var box = elem.getBoundingClientRect()
            var body = document.body
            var docElem = document.documentElement
            var scrollTop = window.pageYOffset || docElem.scrollTop || body.scrollTop
            var scrollLeft = window.pageXOffset || docElem.scrollLeft || body.scrollLeft
            var clientTop = docElem.clientTop || body.clientTop || 0
            var clientLeft = docElem.clientLeft || body.clientLeft || 0
            var top = box.top + scrollTop - clientTop
            var left = box.left + scrollLeft - clientLeft
            return { top: Math.round(top), left: Math.round(left) }
        }

        // Caption position handler
        gallery.listen('afterChange', function () {
            var caption = document.getElementById('cascade-caption');
            var imgContainer = document.getElementsByClassName('pswp__container')[0];
            var citem = gallery.currItem.el; // thumbnail
            if (citem) {
                var txt = '';
                var pos = getOffsetRect(citem);
                txt += 'x=' + pos.top + ', y=' + pos.left + '\n\r';

                //for (var i = 0; i < imgContainer.children.length; i++) {
                //    var pos = getOffsetRect(imgContainer.children[i]);
                //    txt += 'i=' + i + ', x=' + pos.top + ', y=' + pos.left + '\n\r';
                //}
                //alert(gallery.viewportSize);
            }
            //var rect = imgContainer.get
        });

        //////////////////////////////////////// Responsive images -- DAG added: ////////////////////////////////////////////
        //
        // This code sets up PhotoSwipe to request images from ImageResizer. It works by calculating an ideal image size, 
        // given the size of the current viewport and the actual maximum image size available on the server (and stored in 
        // item.sizew). Sizes are rounded to the next 100 pixels in width to reduce impact on the server and intermediate 
        // caches (impacts the Browser cache, CDN, and ImageResizer cache).
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // create variable that will store real size of viewport
        var realViewportWidth,
            realViewportHeight,
            useLargeImages = false,
            firstResize = true,
            imageSrcWillChange;


        // beforeResize event fires each time size of gallery viewport updates
        gallery.listen('beforeResize', function () {
            // gallery.viewportSize.x - width of PhotoSwipe viewport
            // gallery.viewportSize.y - height of PhotoSwipe viewport
            // window.devicePixelRatio - ratio between physical pixels and device independent pixels (Number)
            //                          1 (regular display), 2 (@2x, retina) ...


            // calculate real pixels when size changes
            //realViewportWidth = gallery.viewportSize.x * window.devicePixelRatio;
            realViewportHeight = gallery.viewportSize.y * window.devicePixelRatio;
            gallery.invalidateCurrItems();
        });

        // Break points for image sizes are based on the image width: 100 200 400 800 1200 1600 2400 3200
        // Only images of the following widths are requested
        // Minimizes cache overload and excessive server requests during browser resizing
        //var getBreakpointImageWidth = function (x) {
        //    if (x <= 100)
        //        return 100;
        //    else if (x <= 200)
        //        return 200;
        //    else if (x <= 400)
        //        return 400;
        //    else if (x <= 800)
        //        return 800;
        //    else if (x <= 1200)
        //        return 1200;
        //    else if (x <= 1600)
        //        return 1600;
        //    else if (x <= 2400)
        //        return 2400;
        //    else if (x <= 3200)
        //        return 3200;
        //    return x;
        //}

        var getBreakpointImageHeight = function (y) {
            if (y <= 100)
                return 100;
            else if (y <= 200)
                return 200;
            else if (y <= 400)
                return 400;
            else if (y <= 800)
                return 800;
            else if (y <= 1200)
                return 1200;
            else if (y <= 1600)
                return 1600;
            else if (y <= 2400)
                return 2400;
            else if (y <= 3200)
                return 3200;
            return y;
        }

        var limitItemSize = function (item) {

            // reduce item.h and item.w to fit within item.limith and item.limitw
            // while preserving the aspect ratio

            // if no limit specified then return
            if (item.limith <= 0 && item.limitw <= 0)
                return;

            var factorw = item.limitw / item.w;
            var factorh = item.limith / item.h;

            if (factorw < 1.0 || factorh < 1.0) {
                var factor = 1.0;

                // get smallest non-zero factor
                if (factorw <= 0)
                    factor = factorh;
                else if (factorh <= 0)
                    factor = factorw;
                else
                    factor = factorh < factorw ? factorh : factorw;

                // reduce accordingly
                item.h = item.h * factor;
                item.w = item.w * factor;
            }
        }

        // gettingData event fires each time PhotoSwipe retrieves image source & size
        gallery.listen('gettingData', function (index, item) {

            if (!item.src)
                return;

            var aspect = item.sizew / item.sizeh;

            // get ideal required image size
            //item.w = Math.min(getBreakpointImageWidth(realViewportWidth), item.sizew);
            //item.h = item.w * aspect;

            item.h = Math.min(getBreakpointImageHeight(realViewportHeight), item.sizeh);
            item.w = item.h * aspect;

            // limit to size specified
            limitItemSize(item);

            // append/update ImageResizer query string parameters to request an image of the right size
            var comps = item.src.split('?');
            var src = comps[0];
            var qs = comps[1];
            var qsitems = []; // array of querystring 'parameter=value' strings

            if (qs) {

                qsitems = qs.split('&');

                // remove height, width from list of qs items, starting from end to avoid indexing issues
                for (var i = qsitems.length - 1; i >= 0; --i) {
                    var name = qsitems[i].split('=')[0].trim().toLowerCase();
                    if (name == 'width' || name == 'height') {
                        qsitems.splice(i, 1);
                    }
                }
            }
            // add height / width
            // NB: ~~ converts double to int (~= floor())
            qsitems.push('width=' + ~~item.w);
            qsitems.push('height=' + ~~item.h);
            qs = qsitems.join('&');

            // reassemble src with Image Resizer querystring
            item.src = src + '?' + qs;

        });

        ///////////////////////// end of Responsive Code /////////////////////////////////////

        gallery.init();
    };

    // loop through all gallery elements and bind events
    var galleryElements = document.querySelectorAll(gallerySelector);

    for (var i = 0, l = galleryElements.length; i < l; i++) {
        galleryElements[i].setAttribute('data-pswp-uid', i + 1);
        galleryElements[i].onclick = onThumbnailsClick;
    }

    // Parse URL and open gallery if it contains #&pid=3&gid=1
    var hashData = photoswipeParseHash();
    if (hashData.pid > 0 && hashData.gid > 0) {
        openPhotoSwipe(hashData.pid - 1, galleryElements[hashData.gid - 1], true);
    }
};

