// Rich Text Editor Functions
window.applyFormat = function (format) {
    const selection = window.getSelection();
    
    if (!selection.rangeCount || selection.isCollapsed) {
        console.warn('No text selected');
        return;
    }

    const range = selection.getRangeAt(0);
    
    try {
        switch (format) {
            case 'bold':
                document.execCommand('bold', false, null);
                break;
            case 'italic':
                document.execCommand('italic', false, null);
                break;
            case 'underline':
                document.execCommand('underline', false, null);
                break;
            case 'h1':
                document.execCommand('formatBlock', false, '<h1>');
                break;
            case 'h2':
                document.execCommand('formatBlock', false, '<h2>');
                break;
            case 'h3':
                document.execCommand('formatBlock', false, '<h3>');
                break;
            case 'ul':
                document.execCommand('insertUnorderedList', false, null);
                break;
            case 'ol':
                document.execCommand('insertOrderedList', false, null);
                break;
            default:
                console.warn('Unknown format:', format);
        }
    } catch (error) {
        console.error('Error applying format:', error);
    }
};

window.getEditorContent = function (editorRef) {
    if (editorRef) {
        return editorRef.innerHTML || '';
    }
    return '';
};

window.setEditorContent = function (editorRef, content) {
    if (editorRef) {
        editorRef.innerHTML = content || '';
    }
};

window.editor = {
    exec: function (command) {
        document.execCommand(command, false, null);
    },

    formatBlock: function (tag) {
        document.execCommand("formatBlock", false, tag);
    },

    getHtml: function (el) {
        return el.innerHTML;
    },

    setHtml: function (el, html) {
        el.innerHTML = html;
    }
};
