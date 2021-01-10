function updateItems() {
  document.getElementById('btnUpdate').click();
}

function deleteArticle(id) {
  document.getElementById('btnDeleteFor_' + id).click();
}

function updateItem(id) {
  let itemPrice = parseFloat(document.getElementById('itemPrice_' + id).innerText.replace('€ ', ''));
  let optionCheckField = document.getElementById('chkOption0_' + id);
  let optionSelectField = document.getElementById('selOption0_' + id);
  let extraPrice = 0;
  let iterator = 0;

  while (iterator < 2 || optionCheckField !== null || optionSelectField !== null) {
    let checkPriceField = document.getElementById('extraPrice' + iterator + '_' + id);

    if (optionSelectField !== null)  {
      splitPriceField = optionSelectField.selectedOptions[0].innerText.split('€ ')[1];

      if (splitPriceField !== null && parseFloat(splitPriceField) > 0.0) {
        extraPrice += parseFloat(splitPriceField);
      }
    }
    else if (checkPriceField !== null && optionCheckField.checked) {
      extraPrice += parseFloat(checkPriceField.innerText);
    }

    iterator++;
    optionCheckField = document.getElementById('chkOption' + iterator + '_' + id);
    optionSelectField = document.getElementById('selOption' + iterator + '_' + id);
  }

  document.getElementById('itemTotal_' + id).innerText = '€ ' + ((itemPrice + extraPrice) * document.getElementById('selQuantityFor_' + id).value).toFixed(2);
  sumAllPrices();
}

function sumAllPrices() {
  let totalFields = document.querySelectorAll('.itemTotal');
  let quantityFields = document.querySelectorAll('.itemQuantity');
  let grandTotal = 0;
  let totalNumber = 0;

  for (x = 0; x < totalFields.length; x++) {
    let subTotalPrice = parseFloat(totalFields[x].innerText.replace('€ ', ''));
    grandTotal += subTotalPrice;
  }

  for (x = 0; x < quantityFields.length; x++) {
    totalNumber += parseInt(quantityFields[x].value);
  }

  document.getElementById('shopcartQuantity').innerText = totalNumber;
  document.getElementById('shopcartQuantitySmall').innerText = totalNumber;
  document.getElementById('grandTotal').innerText = '€ ' + grandTotal.toFixed(2);
  document.getElementById('shopcartTotal').innerText = grandTotal.toFixed(2);
  document.getElementById('shopcartTotalSmall').innerText = grandTotal.toFixed(2);
}
