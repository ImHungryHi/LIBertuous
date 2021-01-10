function updateItems() {
  document.getElementById('btnBrowse').click();
}

function chkMultiChecked (chkBox) {
  var hideables = document.getElementsByClassName('hideable');
  var txtClassName = 'hideable';

  if (chkBox.checked) {
    txtClassName = 'field hideable';
  }
  else {
    txtClassName = 'field hideable hidden';
  }

  Array.from(hideables).forEach(elmnt => elmnt.className = txtClassName);
}
