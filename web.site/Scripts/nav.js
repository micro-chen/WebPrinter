
(function () {
    //Ensure body loaded
    if (!document.body)
        return setTimeout(arguments.callee, 50);

    //Variables
    var contentIndex = -1, currentObj = {};
    var em = 0;

    //Binding event handler
    // window.onresize = Resizing;

    var contents = document.getElementById('sf-menu').children;
    for (var i = 0; i < contents.length; i++) {
        contents[i].index = i;
        contents[i].onmouseenter = ContentMouseHandler;
        contents[i].onmouseleave = ContentMouseHandler;
    }

    //Init
    // Resizing();

    //-------------------------------------------------------------------------------------------------

    //Set fontSize
    // function Resizing() {
    //     var de = document.body;
    //     var wi = de.clientWidth;
    //     if (wi < 1200)
    //         wi = 1200;
    //     de.style.fontSize = wi / 20 + 'px';
    //     em = wi/20;
    //     FixS1();
    // }

    //Display animation of content while mouse entering or leaving
    function ContentMouseHandler(e) {
        var type = (event.type == "mouseenter") ? 0 : 1;
        if(type == 0){
            var className = "header-title full ";
            if(contentIndex != -1){
                className += (contentIndex < this.index) ? "flr" : "fle";
            }
            this.className = className;
            contentIndex = this.index;
        }else{
            currentObj = this;
            setTimeout(function(){
                var className = "empty header-title ";
                if(contentIndex != currentObj.index){
                    className += (contentIndex < currentObj.index) ? "flr" : "fle";
                }else{
                    contentIndex = -1;
                }
                currentObj.className = className;
            }, 10);
        }
    }

    // function FixS1(){
    //     var s1 = document.getElementById('s1');
    //     var s1Height = s1.clientHeight;
    //     if(s1Height > 8.3 * em){
    //         document.getElementById('s1-content').style.paddingTop = ((s1Height - 8.3 * em) / 3) + 'px';
    //     }
    // }

})();
