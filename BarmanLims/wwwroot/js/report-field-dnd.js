window.reportFieldDnD = {

    draggedCode: null,

    init: function (dotNetHelper) {

        document.querySelectorAll(
            '[data-report-field-drag]'
        ).forEach(function (element) {

            element.addEventListener(
                'dragstart',
                function (event) {

                    const code =
                        element.getAttribute(
                            'data-report-field-drag');

                    window.reportFieldDnD.draggedCode = code;

                    event.dataTransfer.effectAllowed = 'move';
                    event.dataTransfer.setData(
                        'text/plain',
                        code);

                    element.classList.add(
                        'report-field-dragging');
                });

            element.addEventListener(
                'dragend',
                function () {

                    element.classList.remove(
                        'report-field-dragging');

                    window.reportFieldDnD.draggedCode = null;
                });

            element.addEventListener(
                'dragover',
                function (event) {

                    event.preventDefault();

                    event.dataTransfer.dropEffect =
                        'move';
                });

            element.addEventListener(
                'drop',
                function (event) {

                    event.preventDefault();
                    event.stopPropagation();

                    const targetCode =
                        element.getAttribute(
                            'data-report-field-drag');

                    const sourceCode =
                        event.dataTransfer.getData(
                            'text/plain');

                    if (!sourceCode ||
                        !targetCode ||
                        sourceCode === targetCode) {
                        return;
                    }

                    dotNetHelper.invokeMethodAsync(
                        'ReorderFieldFromJavaScript',
                        sourceCode,
                        targetCode);
                });
        });
    }
};
