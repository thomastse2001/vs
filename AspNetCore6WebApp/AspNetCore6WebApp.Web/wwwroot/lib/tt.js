function setCascadingDropdownItems(e, dropdownId, handlerName, parameterName, valueProperty, displayProperty) {
    document.getElementById(dropdownId).innerHTML = "";
    fetch(`?handler=${handlerName}&${parameterName}=${e.target.value}`)
        .then(function(response) {
            return response.json();
        })
        .then(function(data) {
            Array.prototype.forEach.call(data, function (item, i) {
                document.getElementById(dropdownId).innerHTML += `<option value="${item[valueProperty]}">${item[displayProperty]}</option>`;
            });
        });
}
