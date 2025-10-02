
class HtmxTable {

    static clearFilter(filterElement) {
        console.log("Clearing property");
        if (!filterElement)
            return false;

        filterElement.value = null;
        filterElement.setAttribute('data-filter', "");
        HtmxTable.#addFilterToSortColumns(filterElement);

        return false;
    }

    static setFilter(filterElement) {
        console.log("Setting Data-Filter property");
        if (!filterElement)
            return false;

        const value = filterElement.value;
        if (value == undefined || value == null)
            filterElement.setAttribute('data-filter', "");
        else
            filterElement.setAttribute('data-filter', value);

        HtmxTable.#addFilterToSortColumns(filterElement);

        return false;
    }

    // Checks to see if user cleared out search critera and if so, append query parameter
    static checkFilterCriteria(filterElement, parameterValue) {
        console.log("Checking Filter Criteria");
        var ret = null;

        if (!filterElement)
            return ret;

        const value = filterElement.value;
        if (value == undefined || value == null || value == "")
            ret = parameterValue;

        return ret;
    }


    static flipColumnSortOrder(columnElement, filterElement) {
        console.log("Flipping Column Sort Order");
        if (!columnElement)
            return false;

        var sortOrder = columnElement.getAttribute('data-sortOrder');
        if (sortOrder) {
            if (sortOrder == "asc")
                columnElement.setAttribute('data-sortOrder', "desc");
            else
                columnElement.setAttribute('data-sortOrder', "asc");

            const queryParams = HtmxTable.createColumnSortParameters(columnElement, filterElement);
            HtmxTable.setAnchorUrl(columnElement, queryParams);
        }

        return false; 
    }


    static createColumnSortParameters(columnElement, filterElement) {
        console.log("Creating Column Sort Parameters");
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

    static setAnchorUrl(element, url) {
        console.log("Setting Anchor URL");

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


    static #addFilterToSortColumns(filterElement) {
        console.log("Adding filter to sort columns");
        const sortColumns = document.getElementsByName("sortColumn");
        sortColumns.forEach((sortColumnElement, index) => {
            const queryParams = HtmxTable.createColumnSortParameters(sortColumnElement, filterElement);
            HtmxTable.setAnchorUrl(sortColumnElement, queryParams);
        });
    }
}