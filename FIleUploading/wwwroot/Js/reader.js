﻿function base64ToArrayBuffer(base64String) {
    var binary_string = window.atob(base64String);
    var len = binary_string.length;
    var bytes = new Uint8Array(len);

    for (var i = 0; i < len; i++) {
        bytes[i] = binary_string.charCodeAt(i);
    }

    return bytes.buffer;
}

function loadVideo(base64String, contentType) {
    var arrayBuffer = base64ToArrayBuffer(base64String);

    const blob = new Blob([arrayBuffer], { type: contentType });
    var dataUrl = window.URL.createObjectURL(blob);

    //console.log("the data => " + blob);
    //console.log("the url => " + dataUrl);

    var video = document.getElementById("videoPlayer");
    video.src = "";
    video.src = dataUrl;
    video.play();
}