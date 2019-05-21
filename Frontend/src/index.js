import $ from 'jquery';
import 'bootstrap/dist/css/bootstrap.min.css';
import './styles.css';

let cities = [];

function GetInitialState() {
    $.ajax({
        url: '/api/cities',
        type: 'GET',
        contentType: 'application/json',
        success: (receivedCities) => {
            cities = receivedCities;
            $("form select").append(
                cities.reduce((prev, curr) =>
                    prev + `<option value="${curr.id}">${curr.name}</option>`,
                    '<option value=""></option>')
            );
            GetSales();
        },
        error: function () {
            alert('Что то пошло не так :(');
        }
    });
}

function GetSales() {
    $.ajax({
        url: '/api/sales',
        type: 'GET',
        contentType: 'application/json',
        success: (sales) => {
            const rows = sales.reduce((prev, curr) => {
                return prev + row(curr);
            }, '');
            $("table tbody").append(rows);
        },
        error: function () {
            alert('Что то пошло не так :(');
        }
    });
}

function DeleteSale(id) {
    $.ajax({
        url: `/api/sales/${id}`,
        type: 'DELETE',
        contentType: 'application/json',
        success: () => {
            $(`tr[data-id='${id}']`).remove();
        },
        error: function () {
            alert('Что то пошло не так :(');
        }
    });
}

function CreateSale({name, clientName, contactPerson, saller, cityId}) {
    $.ajax({
        url: "api/sales",
        contentType: "application/json",
        method: "POST",
        data: JSON.stringify({
            name,
            clientName,
            contactPerson,
            saller,
            cityId
        }),
        success: function (sale) {
            $("table tbody").append(row(sale));
            removeFormErrors();
            document.querySelector('form').reset();
        },
        error: function () {
            alert('Что то пошло не так :(');
        }
    })
}

function EditeSale(id, name, clientName, contactPerson, saller, cityId) {
    $.ajax({
        url: `api/sales/${id}`,
        contentType: 'application/json',
        method: 'PUT',
        data: JSON.stringify({
            name,
            clientName,
            contactPerson,
            saller,
            cityId
        }),
        success: function (sale) {
            $(`table tbody tr[data-id='${sale.id}']`).replaceWith(row(sale));
        },
        error: function () {
            alert('Что то пошло не так :(');
        }
    })
}

function editableRowView(id) {
    const tr = document.querySelector(`tr[data-id="${id}"]`);
    tr.classList.remove('not-editable');
    tr.querySelectorAll('input').forEach(elem => elem.disabled = false);
    tr.querySelector('select').disabled = false;
    const butt =  tr.querySelector('button');
    butt.innerHTML = 'Сохранить';
    butt.setAttribute('data-action', 'save');
    butt.classList.remove('btn-secondary');
    butt.classList.add('btn-info');
}

function row(sale) {
    const options = cities.reduce((prev, curr) =>
        prev + `<option value="${curr.id}" ${curr.id === (sale.city && sale.city.id) ? 'selected' : ''}>${curr.name}</option>`,
        '<option value=""></option>');
    return `
<tr class="not-editable" data-id="${sale.id}">
    <td><input name="name" disabled type="text" value="${sale.name}"/></td>
    <td><input name="clientName" disabled type="text" value="${sale.clientName}"/></td>
    <td><input name="contactName" disabled type="text" value="${sale.contactName}"/></td>
    <td><input name="saller" disabled type="text" value="${sale.saller || ''}"/></td>
    <td><select name="cityId" disabled>${options}</select></td>
    <td>
        <button data-action="update" data-id="${sale.id}" class="btn btn-sm btn-secondary">Редактировать</button>
    </td>
    <td>
        <button data-action="delete" data-id="${sale.id}" class="btn btn-sm btn-danger">Удалить</button>
    </td>
</tr>`;
}

function handleTableClick(e) {
    const action = e.target.dataset.action;

    switch (action) {
        case 'delete':
            DeleteSale(e.target.dataset.id);
            break;
        case 'update':
            editableRowView(e.target.closest('tr').dataset.id);
            break;
        case 'save':
            const tr = e.target.closest('tr');
            const id = tr.dataset.id;
            const name = tr.querySelector('input[name="name"]').value;
            const clientName = tr.querySelector('input[name="clientName"]').value;
            const contactPerson = tr.querySelector('input[name="contactName"]').value;
            const saller = tr.querySelector('input[name="saller"]').value;
            const cityId = tr.querySelector('select[name="cityId"]').value;

            const errors = validateData({name});
            let hasErrors = false;
            Object.keys(errors).forEach((key => {
                hasErrors = true;
                $(`table tr[data-id="${id}"] input[name="${key}"]`).closest('td').append(errorMessage(errors[key]));
            }));
            if(hasErrors) return;
            removeTableErrorById(id);
            EditeSale(id, name, clientName, contactPerson, saller, cityId);
            break;
        default:
            return;
    }
}

function isEmptyOrNull(value) {
    return !value || value.trim() === '';
}

function removeFormErrors() {
    $('form span.error').remove();
}

function removeTableErrorById(id) {
    $(`table tr[data-id="${id}"] span.error`).remove();
}

function validateData(data) {
    const { name } = data;
    const errors = {};
    if (isEmptyOrNull(name)){
        errors.name = 'Поле с названием не должно быть пустым';
    }
    return errors;
}

function handleFormSubmit(e) {
    e.preventDefault();
    const elements = this.elements;
    const data = {};
    if (!isEmptyOrNull(elements.name.value)) {
        data.name = elements.name.value;
    }
    if (!isEmptyOrNull(elements.clientName.value)) {
        data.clientName = elements.clientName.value;
    }
    if (!isEmptyOrNull(elements.contactPerson.value)) {
        data.contactPerson = elements.contactPerson.value;
    }
    if (!isEmptyOrNull(elements.saller.value)) {
        data.saller = elements.saller.value;
    }
    if (!isEmptyOrNull(elements.cityId.value)) {
        data.cityId = elements.cityId.value;
    }
    const errors = validateData(data);

    let hasErrors = false;
    Object.keys(errors).forEach((key => {
        hasErrors = true;
        $(`form div[data-field="${key}"]`).append(errorMessage(errors[key]));
    }));
    if(hasErrors) return;

    CreateSale(data);
}

function errorMessage(message) {
    return `<span class="error">${message}</span>`
}

function AddEvents() {
    document.getElementsByTagName('table')[0].addEventListener('click', handleTableClick);
    document.getElementsByTagName('form')[0].addEventListener('submit', handleFormSubmit)
}

GetInitialState();
AddEvents();