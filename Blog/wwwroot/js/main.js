$(function () {
    var output = document.getElementById('UnAuthorization');
    //var buttonSendPost = document.getElementById('buttonSendPost');

    if (output != null) {
        output.onclick = function () {
            $.ajax({
                type: "POST",
                url: "Home/UnAuthenticated",
                //data: "{'param1': 'Fp1', 'param2': 'Sp2'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true, cache: false,
                success: function (data, textStatus, XHR) {
                    location.reload();
                },
                error: function () {
                }
            });
            return false;
        }
    }

    //if (buttonSendPost != null) {
    //    buttonSendPost.onclick = function () {
    //        console.log(buttonSendPost);
    //        const form = document.getElementById('addPost');
    //        const titlepost = form.querySelector('[name="titlepost"]').value,
    //            textpost = form.querySelector('[name="textpost"]').value,
    //            photopost = form.querySelector('[name="photopost"]').value;
    //        console.log(photopost);
    //    }
    //}
});

//function openFormAddPost() {
//    document.getElementById("addPost").style.display = "block";
//}

//function closeFormAddPost() {
//    document.getElementById("addPost").style.display = "none";
//}