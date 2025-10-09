class HtmxUtils {
    static #valueSeparator = '|'

    static handleToggle(evt, toggledElement) {
        if (!toggledElement)
            throw new Error("Toggle Element must be a valid");

        const toggleStateStringValues = toggledElement.getAttribute('data-toggle-state-values');
        if (!toggleStateStringValues)
            throw new Error("Toggle Element missing list of state values (data-toggle-state-values)");

        const toggleStateArrayValues = toggleStateStringValues.split(HtmxUtils.#valueSeparator);
        if (toggleStateArrayValues.length != 2)
            throw new Error("Toggle Element state values must have 2 values separated by '" + HtmxUtils.#valueSeparator + "' (data-toggle-state-values)");

        var currentToggleState = toggledElement.getAttribute('data-toggle-state');
        if (!currentToggleState)
            currentToggleState = toggleStateArrayValues[0];    // If not set, assume first value in list


        // Determine current state within list of possible values
        var foundIndex = 0;
        for (var i = 0; i < toggleStateArrayValues.length; i++) {
            if (currentToggleState === toggleStateArrayValues[i]) {
                foundIndex = i;
                break;
            }
        }


        // Switch states and save back to element
        var newToggleState = (foundIndex == 0 ? toggleStateArrayValues[1] : toggleStateArrayValues[0]);
        toggledElement.setAttribute('data-toggle-state', newToggleState);


        // Raise event for HTMX
        var eventName = "toggle-" + newToggleState;
        htmx.trigger(toggledElement, eventName, { toggleState: newToggleState });
    }
}
