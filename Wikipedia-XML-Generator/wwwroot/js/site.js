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
            "DTD": editors["input-dtd"].getValue(),
            "XML": editors["input-xml"].getValue(),
        };
    }

    async function editorOnChangeHandler(cm, change) {
        var data = await getModel();

        var editorName = cm.options["name"];

        $('#input-' + editorName).val(cm.getValue());

        var onload = () => {
            var xhr = event.target;
            if (xhr.responseText != "true" && cm.getValue() != '') {
                $('#input-' + editorName).nextAll('.CodeMirror').css('border', '2px solid red');
            }
            else {
                $('#input-' + editorName).nextAll('.CodeMirror').css('border', '');
            }
        }

        await sendRequest('POST', '/validator/validate' + editorName, data, onload);
    }

    $('.doc-input').each((i, v) => {
        var editor = CodeMirror.fromTextArea(v, {
            name: ["dtd", "xml"][i],
            lineNumbers: true,
            mode: ["dtd", "xml"][i],
            theme: 'abbott',
            lineWrapping: $("#switchWrap").prop('checked'),
        });

        editor.on("change", editorOnChangeHandler);
        editors[v.id] = editor;
    });

    $('#btnBeautify').on('click', async () => {
        var edited = html_beautify(editors["input-xml"].getValue(), {
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

        editors["input-xml"].getDoc().setValue(edited);
    });

    $('.CodeMirror').each((i, e) => {
        switch (e.previousSibling.id) {
            case "input-dtd": {
                $(e).find('textarea').attr('name', 'DTD');
                break;
            }
            case "input-xml": {
                $(e).find('textarea').attr('name', 'XML');
                break;
            }
        }
    });

    $("#switchWrap").change(() => {
        for (var k in editors) {
            editors[k].setOption('lineWrapping', $("#switchWrap").prop('checked'));
        }
    });

    $("textarea").trigger("change");
});
