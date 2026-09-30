initSortable = function (element, dotNetHelper) {
    if (!element) return;

    Sortable.create(element, {
        animation: 150,
        direction: 'horizontal',
        draggable: '.tab-item',
        filter: 'button',
        preventOnFilter: false,
        onEnd: function (evt) {
            dotNetHelper.invokeMethodAsync('ReorderTabs', evt.oldIndex, evt.newIndex);
        }
    });
};