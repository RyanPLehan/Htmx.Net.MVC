; (function () {
    //htmx.logAll();
})()

function clearFilter(filterElement) {
    if (!filterElement)
        return false;

    filterElement.value = null;
    filterElement.setAttribute('data-filter', "");

    const sortColumns = document.getElementsByName("sortColumn");
    sortColumns.forEach((sortColumnElement, index) => {
        const queryParams = createColumnSortParameters(sortColumnElement, filterElement);
        setAnchorUrl(sortColumnElement, queryParams);
    });

    return false;
}

function setFilter(filterElement) {
    if (!filterElement)
        return false;

    const value = filterElement.value;
    if (value == undefined || value == null)
        filterElement.setAttribute('data-filter', "");
    else
        filterElement.setAttribute('data-filter', value);

    const sortColumns = document.getElementsByName("sortColumn");
    sortColumns.forEach((sortColumnElement, index) => {
        const queryParams = createColumnSortParameters(sortColumnElement, filterElement);
        setAnchorUrl(sortColumnElement, queryParams);
    });

    return false;
}

// Checks to see if user cleared out search critera and if so, append query parameter
function checkFilterCriteria(filterElement, parameterValue) {
    var ret = null;

    if (!filterElement)
        return ret;

    const value = filterElement.value;
    if (value == undefined || value == null || value == "")
        ret = parameterValue;

    return ret;
}


function flipColumnSortOrder(columnElement, filterElement) {
    if (!columnElement)
        return false;

    var sortOrder = columnElement.getAttribute('data-sortOrder');
    if (sortOrder) {
        if (sortOrder == "asc")
            columnElement.setAttribute('data-sortOrder', "desc");
        else
            columnElement.setAttribute('data-sortOrder', "asc");

        const queryParams = createColumnSortParameters(columnElement, filterElement);
        setAnchorUrl(columnElement, queryParams);
    }

    return false; 
}


function createColumnSortParameters(columnElement, filterElement) {
    if (!columnElement)
        return '';

    const columnName = columnElement.getAttribute('data-columnName');
    const sortOrder = columnElement.getAttribute('data-sortOrder');

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

    const href = element.getAttribute('href');
    if (href != undefined && href != null)
        element.setAttribute('href', url);

    const hxget = element.getAttribute('hx-get');
    if (hxget != undefined && hxget != null)
        element.setAttribute('hx-get', url);

    htmx.process(element);

    return false;
}

