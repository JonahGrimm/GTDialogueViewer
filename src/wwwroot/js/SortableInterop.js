window.sortableInterop = {
    init: function (elementId, dotNetHelper) {
        var el = document.getElementById(elementId);
        if (!el) return;

        Sortable.create(el, {
            animation: 150,
            onEnd: function (evt) {
                // Notify Blazor when dragging finishes
                dotNetHelper.invokeMethodAsync('OnReorder', evt.oldIndex, evt.newIndex);
            }
        });
    }
};