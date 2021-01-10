function sortAlfa() {
  document.getElementById('btnSortAlfa').click();
}

function sortPrice() {
  document.getElementById('btnSortPrice').click();
}

function submitForm() {
  document.getElementById('btnSubmit').click();
}

function sendItem(productId) {
  event.preventDefault();
  productId = parseInt(productId);
  let quantity = parseInt(document.getElementById('txtQuantity_' + productId).value);
  let price = parseFloat(document.getElementById('priceSpan_' + productId).innerText.replace('€ ', ''));
  let totalPriceField = document.getElementById('shopcartTotal');
  let totalQuantityField = document.getElementById('shopcartQuantity');
  let totalPrice = 0;
  let totalQuantity = 0;
  let getParams = 'id=' + productId + '&quantity=' + quantity;
  let choiceIds = [];
  let loopThrough = true;
  let x = 0;

  if (quantity > 1000) {
    quantity = 1000;
  }

  if (totalPriceField !== null && totalQuantityField !== null) {
    totalPrice = parseFloat(totalPriceField.innerText);
    totalQuantity = parseInt(totalQuantityField.innerText);
  }

  while (loopThrough) {
    let chkBoxElement = document.getElementById('chkOption' + x + '_' + productId);
    let selElement = document.getElementById('selOption' + x + '_' + productId);

    if (x > 0 && (chkBoxElement === null && selElement === null)) {
      // Cut out of the loop - tell the admin to clean up his data
      loopThrough = false;
    }
    else if (chkBoxElement !== null && chkBoxElement.checked && parseInt(chkBoxElement.value) > 0) {
      // Some checkbox additions
      if (x > 1) {
        choiceIds.push(parseInt(chkBoxElement.value));
      }

      getParams += '&options[]=' + x + 'x' + parseInt(chkBoxElement.value);
    }
    else if (selElement !== null && parseInt(selElement.value) > 0) {
      // Some dropdown additions
      if (x > 1) {
        choiceIds.push(parseInt(selElement.value));
      }

      getParams += '&options[]=' + x + 'x' + parseInt(selElement.value);
    }

    x++;
  }

  fetch('addToCart.php?' + getParams);
  totalQuantity += quantity;
  totalQuantityField.innerText = totalQuantity;
  document.getElementById('shopcartQuantitySmall').innerText = totalQuantity;

  if (totalQuantity > 1) {
    document.getElementById('shopcartPlural').innerText = 'en';
  }
  else {
    document.getElementById('shopcartPlural').innerText = '';
  }

  if (document.getElementById('shopcartLayout').className = 'hidden') {
    document.getElementById('shopcartLayout').className = 'columns is-12 is-clearfix';
  }

  if (document.getElementById('shopcartQuantitySmallDiv').className = 'hidden') {
    document.getElementById('shopcartQuantitySmallDiv').className = 'is-pulled-left';
  }

  if (document.getElementById('shopcartTotalSmallDiv').className = 'hidden') {
    document.getElementById('shopcartTotalSmallDiv').className = 'is-pulled-right';
  }

  // This is the only thing that needs to be done in a delayed manner, since the only reason we need prices is to update the cart
  if (choiceIds.length > 0) {
    fetch('getOptionPrices.php?ids[]=' + choiceIds.join('&ids[]='))
    .then((res) => {
      res.json().then(function(data) {
        let resultPrices = data;

        resultPrices.forEach(fltPrice => price += parseFloat(fltPrice['price']));

        totalPrice += (price * quantity);
        totalPriceField.innerText = totalPrice.toFixed(2);
        document.getElementById('shopcartTotalSmall').innerText = totalPrice.toFixed(2);
      });
    })
    .catch((error) => console.log('error: ' + error));
  }
  else {
    // Anything else? Just update the shopcart with the known product price
    totalPrice += (price * quantity);
    totalPriceField.innerText = totalPrice.toFixed(2);
    document.getElementById('shopcartTotalSmall').innerText = totalPrice.toFixed(2);
  }
}

function filterByText() {
  let filterField = document.getElementById('txtFilterText');

  if (filterField !== null) {
    let filterContent = filterField.value;
    let allArticles = document.querySelectorAll('article.column');

    if (filterContent !== '' && allArticles !== null && allArticles !== []) {
      let arrFilteredArticles = Array.from(allArticles).filter(x => x.innerText.toLowerCase().includes(filterContent.toLowerCase()));
      let arrOtherArticles = Array.from(allArticles).filter(x => !x.innerText.toLowerCase().includes(filterContent.toLowerCase()));
      arrOtherArticles.forEach(x => x.className = 'column is-one-third is-hidden');
      arrFilteredArticles.forEach(x => x.className = 'column is-one-third');
    }
  }
}
