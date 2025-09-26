; (function () {
    //htmx.logAll();
})()

function clearFilter(filterElement) {
    if (!filterElement)
        return false;

    filterElement.value = null;
    filterElement.setAttribute('data-filter', "");

    var sortColumns = document.getElementsByName("sortColumn");
    sortColumns.forEach((sortColumnElement, index) => {
        var queryParams = createColumnSortParameters(sortColumnElement, filterElement);
        setAnchorUrl(sortColumnElement, queryParams);
    });

    return false;
}

function setFilter(filterElement) {
    if (!filterElement)
        return false;

    var value = filterElement.value;
    if (value == undefined || value == null)
        filterElement.setAttribute('data-filter', "");
    else
        filterElement.setAttribute('data-filter', value);

    var sortColumns = document.getElementsByName("sortColumn");
    sortColumns.forEach((sortColumnElement, index) => {
        var queryParams = createColumnSortParameters(sortColumnElement, filterElement);
        setAnchorUrl(sortColumnElement, queryParams);
    });

    return false;
}


function flipColumnSortOrder(columnElement) {
    if (!columnElement)
        return false;

    var sortOrder = columnElement.getAttribute('data-sortOrder');
    if (sortOrder) {
        if (sortOrder == "asc")
            columnElement.setAttribute('data-sortOrder', "desc");
        else
            columnElement.setAttribute('data-sortOrder', "asc");

        var queryParams = createColumnSortParameters(columnElement, document.getElementById("search-name"));
        setAnchorUrl(columnElement, queryParams);
    }

    return false; 
}


function createColumnSortParameters(columnElement, filterElement) {
    if (!columnElement)
        return '';

    var columnName = columnElement.getAttribute('data-columnName');
    var sortOrder = columnElement.getAttribute('data-sortOrder');

    var filter = "";
    if (filterElement) {
        filter = filterElement.getAttribute('data-filter');
        if (filter == undefined || filter == null)
            filter = "";
    }

    return `?columnName=${columnName}&sortOrder=${sortOrder}&filter=${filter}`;
}

function setAnchorUrl(element, url) {
    console.log(`Element: ${element}`);
    console.log(`Url: ${url}`);

    if (!element)
        return;

    var href = element.getAttribute('href');
    if (href != undefined && href != null)
        element.setAttribute('href', url);

    var hxget = element.getAttribute('hx-get');
    if (hxget != undefined && hxget != null)
        element.setAttribute('hx-get', url);

    htmx.process(element);

    return false;
}

