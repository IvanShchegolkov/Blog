$(function () {
    var a = document.getElementById('UnAuthorization');

    a.onclick = function () {
        $.ajax({
            type: "POST",
            url: "Home/UnAuthenticated",
            //data: "{'param1': 'Fp1', 'param2': 'Sp2'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true, cache: false,
            success: function (XHR) {
                
            }
        });
        return false;
    }
});