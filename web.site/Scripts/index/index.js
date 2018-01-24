$(document).ready(function () {
    $('#layerslider').layerSlider({
        skinsPath: 'layerslider/skins/',
        animateFirstLayer: true,
        navPrevNext: true,
        navStartStop: false,
        navButtons: false,
        autoPlayVideos: false,
        skin: 'minimal'
    });
});
$(function () {


    //getupdate();
    //SetHits();
    //setInterval("SetHits()", 60000); //每隔10秒刷新点击量

    //手机端优化
    if( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent) ) {
        $('.footer .container_12').height($(document).height() - $("#footer").offset().top );
        $('#gg').width($('#footer').width());
    }

    

})

//function SetHits() { //获取最新点击量
//    $.post('GetList.ashx', {
//        _rid: Math.random()
//    },
//    function (str) {
//        $('#dynamicInfo').html(str);
//    });
//}
//function getupdate() {
//    $.post('log.aspx', {
//        visitType: 1
//    },
//    function (data) {
//        $('#version').html(data.version);
//        $('#updatedate').html(data.updatedate);
//        $('#datasize').html(data.datasize);
//    },
//    'json')
//}