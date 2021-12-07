$(document).ready(function () {

    async function sendRequest(path, data, onload) {
        var xhr = new XMLHttpRequest();
        xhr.open("POST", path);
        xhr.setRequestHeader("Accept", "application/json");
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.onload = onload;
        xhr.send(JSON.stringify(data));
    }

    var editors = {};

    async function editorOnChangeHandler(cm, change) {
        var data = {
            "DTD": editors["inputDtd"].getValue(),
            "XML": cm.getValue(),
            "WikiPage": $('#inputWiki').val()
        };

        var onload = () => {
            var xhr = event.target;
            console.log(xhr.responseText);
            if (xhr.responseText != "true" && cm.getValue() != '') {
                $('#inputXml').nextAll('.CodeMirror').css('border', '2px solid red');
            }
            else {
                $('#inputXml').nextAll('.CodeMirror').css('border', '');
            }
        }

        await sendRequest('/validator/validate', data, onload);
    }

    $('.doc-input').each((i, v) => {
        var editor = CodeMirror.fromTextArea(v, {
            lineNumbers: true,
            mode: 'text/x-perl',
            theme: 'abbott',
        });

        if (v.id == "inputXml") {
            editor.on("change", editorOnChangeHandler);
        }
        editors[v.id] = editor;
    });
});