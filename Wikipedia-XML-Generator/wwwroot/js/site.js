$(document).ready(function () {

    async function sendRequest(method, path, data, onload) {
        var xhr = new XMLHttpRequest();
        xhr.open(method, path);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.onload = onload;
        xhr.send(JSON.stringify(data));
    }

    var editors = {};

    async function getModel() {
        return {
            "DTD": editors["inputDtd"].getValue(),
            "XML": editors["inputXml"].getValue(),
            "WikiPage": $('#inputWiki').val()
        };
    }

    async function editorOnChangeHandler(cm, change) {
        var data = await getModel();

        var onload = () => {
            var xhr = event.target;
            if (xhr.responseText != "true" && cm.getValue() != '') {
                $('#inputXml').nextAll('.CodeMirror').css('border', '2px solid red');
            }
            else {
                $('#inputXml').nextAll('.CodeMirror').css('border', '');
            }
        }

        await sendRequest('POST', '/validator/validate', data, onload);
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

    $('#inputDtdFile').on('input', async () => {
        var file = event.target.files[0];
        var fileReader = new FileReader();

        fileReader.onload = (loadedEvent) => {
            var contents = loadedEvent.target.result;
            editors["inputDtd"].getDoc().setValue(contents);
        }

        fileReader.readAsText(file, "UTF-8");
    });

    $('#btnBeautify').on('click', async () => {
        var edited = html_beautify(editors["inputXml"].getValue(), {
            "indent_size": "4",
            "indent_char": " ",
            "max_preserve_newlines": "5",
            "preserve_newlines": true,
            "keep_array_indentation": false,
            "break_chained_methods": false,
            "indent_scripts": "normal",
            "brace_style": "collapse",
            "space_before_conditional": true,
            "unescape_strings": false,
            "jslint_happy": false,
            "end_with_newline": false,
            "wrap_line_length": "0",
            "indent_inner_html": false,
            "comma_first": false,
            "e4x": false,
            "indent_empty_lines": false
        });

        editors["inputXml"].getDoc().setValue(edited);
    });
});