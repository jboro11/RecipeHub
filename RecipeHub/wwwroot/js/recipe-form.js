(() => {
    const form = document.getElementById('recipe-form');
    const container = document.getElementById('ingredients-list');
    const template = document.getElementById('ingredient-template');
    if (!form || !container || !template) return;

    function refreshValidation() {
        if (!window.jQuery?.validator?.unobtrusive) return;
        const $form = window.jQuery(form);
        $form.removeData('validator').removeData('unobtrusiveValidation');
        window.jQuery.validator.unobtrusive.parse(form);
    }

    function reindex() {
        container.querySelectorAll('.ingredient-row').forEach((row, index) => {
            row.dataset.index = index;
            row.querySelectorAll('input, label, [data-valmsg-for]').forEach(element => {
                for (const attribute of ['name', 'id', 'for', 'data-valmsg-for']) {
                    if (!element.hasAttribute(attribute)) continue;
                    element.setAttribute(attribute, element.getAttribute(attribute)
                        .replace(/Ingredients\[\d+\]/g, `Ingredients[${index}]`)
                        .replace(/Ingredients_\d+__/g, `Ingredients_${index}__`));
                }
            });
        });
        refreshValidation();
    }

    container.addEventListener('click', event => {
        const button = event.target.closest('.remove-ingredient');
        if (!button || container.querySelectorAll('.ingredient-row').length <= 2) return;
        button.closest('.ingredient-row').remove();
        reindex();
    });

    document.getElementById('add-ingredient').addEventListener('click', () => {
        const index = container.querySelectorAll('.ingredient-row').length;
        container.insertAdjacentHTML('beforeend', template.innerHTML.replaceAll('__idx__', index));
        refreshValidation();
        container.lastElementChild.querySelector('input').focus();
    });
})();
