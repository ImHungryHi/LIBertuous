<?php

/**
 * Home controller klasse
 * ------------------------------
 */

/**
 * ------------------------------
 * Includes
 * ------------------------------
 */

  // HomeDB
  require_once 'home.db.php';

class HomeController extends PlonkController
{
	// De verschillende views van de module
	protected $views = array(
    'home'
	);

	// De verschillende acties
	protected $actions = array(
    'home'
  );

	/**
	 * Sorts in a custom order
	 * @return array
	 */
	public function multiplusSorticus($arrToSort, $arrOrder, $strColumn) {
		//Do some stuff
	}

	/**
	 * De view van de homepagina
	 * @return
	 */
	public function showHome()
  {
    /**
     * Hoofdlayout
     */
  		// ken waardes toe aan hun bijhorende variabelen
  		$this->mainTpl->assign('pageTitle', 'Promo Drinks - Bladeren');
  		$this->mainTpl->assign('pageMeta', '<link type="text/css" rel="stylesheet" href="modules/home/css/home.css" />
    <script src="modules/home/js/home.js"></script>');

    /**
     * Paginaspecifieke layout
     */
      // Shopcart filled? Show and parse it with data. Otherwise, hide it.
      $arrShopcart = [];
      $this->mainTpl->assign('shopcartUrl', $_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=shopcart');

      if (OneArchySession::exists('shopcartItems')) {
        $arrShopcart = OneArchySession::get('shopcartItems');
      }

      if (count($arrShopcart) > 0) {
        $shopcartQuantity = 0;
        $shopcartTotal = 0;

        foreach ($arrShopcart as $shopcartItem) {
          if (isset($shopcartItem['id']) && isset($shopcartItem['quantity'])) {
            $productId = intval($shopcartItem['id']);
            $tempQuantity = intval($shopcartItem['quantity']);
            $shopcartQuantity += $tempQuantity;
            $extras = array();
            $tempPrice = floatval(HomeDB::getProductPrice($productId));

            if (isset($shopcartItem['options'])) {
              $extras = $shopcartItem['options'];

              if (count($extras) > 0) {
                foreach ($extras as $extra) {
                  if (intval($extra['id']) > 0) {
                    $tempPrice += floatval(HomeDB::getPriceForOption(intval($extra['id'])));
                  }
                }
              }
            }

            $shopcartTotal += ($tempPrice * $tempQuantity);
          }
        }

        $this->mainTpl->assignOption('oShopcartVisible');
        $this->mainTpl->assign('shopcartQuantity', $shopcartQuantity);
        $this->mainTpl->assign('shopcartTotal', number_format($shopcartTotal, 2, '.', ' '));
        $this->mainTpl->assign('shopcartQuantitySmall', $shopcartQuantity);
        $this->mainTpl->assign('shopcartTotalSmall', number_format($shopcartTotal, 2, '.', ' '));

        if ($shopcartQuantity < 2) {
          $this->mainTpl->assignOption('oShopcartNonPlural');
        }
      }
      else {
        $this->mainTpl->assignOption('oShopcartHidden');
        $this->mainTpl->assign('shopcartQuantity', '0');
        $this->mainTpl->assign('shopcartTotal', '0');
        $this->mainTpl->assign('shopcartQuantitySmall', '0');
        $this->mainTpl->assign('shopcartTotalSmall', '0');
      }

      // opvullen van de form
      $this->pageTpl->assign('formUrl', $_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=home');
      $arrCategories = HomeDB::getCategories();
      $this->pageTpl->setIteration('iCategories');
      $arrProducts = array();

      foreach ($arrCategories as $categoryRow) {
        if (isset($_POST['selCategory']) && $_POST['selCategory'] === $categoryRow['type']) {
          $this->pageTpl->assignIterationOption('oCategorySelected');
        }

        $this->pageTpl->assignIteration('categoryValue', $categoryRow['type']);
        $this->pageTpl->assignIteration('categoryText', $categoryRow['type']);
        $this->pageTpl->refillIteration('iCategories');
      }

      $this->pageTpl->parseIteration('iCategories');

      if (!isset($_POST['selCategory']) || (isset($_POST['selCategory']) && ($_POST['selCategory'] === '' || $_POST['selCategory'] === '-1'))) {
        $arrProducts = HomeDB::getAllProducts();
      }
      else {
        $arrProducts = HomeDB::getProductsByCategory($_POST['selCategory']);
      }

      // Set the values for the other sort fields so we know where to go when clicked again
      $sortAlfaValue = 'noSort';
      $sortPriceValue = 'noSort';

      if (isset($_POST['action']) && isset($_POST['txtFilterText'])) {
        $this->pageTpl->assign('txtFilterText', $_POST['txtFilterText']);
      }
      else {
        $this->pageTpl->assign('txtFilterText', '');
      }

      if (isset($_POST['action']) && $_POST['action'] === 'submit') {
        // action === submit means filtering was done (sauce)
        if (isset($_POST['txtAlfabetSort']) && $_POST['txtAlfabetSort'] === 'highLow') {
          $sortAlfaValue = 'highLow';
        }
        elseif (isset($_POST['txtAlfabetSort']) && $_POST['txtAlfabetSort'] === 'lowHigh') {
          $sortAlfaValue = 'lowHigh';
        }
        else {
          $sortAlfaValue = 'noSort';
        }

        if (isset($_POST['txtPriceSort']) && $_POST['txtPriceSort'] === 'highLow') {
          $sortPriceValue = 'highLow';
        }
        elseif (isset($_POST['txtPriceSort']) && $_POST['txtPriceSort'] === 'lowHigh') {
          $sortPriceValue = 'lowHigh';
        }
        else {
          $sortPriceValue = 'noSort';
        }
      }
      elseif (isset($_POST['action']) && $_POST['action'] === 'sortAlfa') {
        // the alfabetical sort link was clicked - sortPriceValue remains empty
        if (isset($_POST['txtAlfabetSort']) && $_POST['txtAlfabetSort'] === 'highLow') {
          $sortAlfaValue = 'lowHigh';
        }
        elseif (isset($_POST['txtAlfabetSort']) && $_POST['txtAlfabetSort'] === 'lowHigh') {
          $sortAlfaValue = 'highLow';
        }
        else {
          $sortAlfaValue = 'lowHigh';
        }
      }
      elseif (isset($_POST['action']) && $_POST['action'] === 'sortPrice') {
        // the price sort link was clicked - sortAlfaValue remains empty
        if (isset($_POST['txtPriceSort']) && $_POST['txtPriceSort'] === 'highLow') {
          $sortPriceValue = 'lowHigh';
        }
        elseif (isset($_POST['txtPriceSort']) && $_POST['txtPriceSort'] === 'lowHigh') {
          $sortPriceValue = 'highLow';
        }
        else {
          $sortPriceValue = 'lowHigh';
        }
      }

      $tempAlfa = array_column($arrProducts, 'name');
      $tempPrice = array_column($arrProducts, 'price');

      switch ($sortAlfaValue) {
        case 'highLow':
          $this->pageTpl->assign('alfaSortText', 'Hoog naar laag');
          $this->pageTpl->assign('alfaSortClass', 'sort-up');
          array_multisort($tempAlfa, SORT_DESC, $arrProducts);
          break;
        case 'lowHigh':
          array_multisort($tempAlfa, SORT_ASC, $arrProducts);
          $this->pageTpl->assign('alfaSortText', 'Laag naar hoog');
          $this->pageTpl->assign('alfaSortClass', 'sort-down');
          break;
        default:
          $this->pageTpl->assign('alfaSortText', 'Laag naar hoog');
          $this->pageTpl->assign('alfaSortClass', 'sort');
      }

      switch ($sortPriceValue) {
        case 'highLow':
          $this->pageTpl->assign('priceSortText', 'Hoog naar laag');
          $this->pageTpl->assign('priceSortClass', 'sort-up');
          array_multisort($tempPrice, SORT_DESC, $arrProducts);
          break;
        case 'lowHigh':
          array_multisort($tempPrice, SORT_ASC, $arrProducts);
          $this->pageTpl->assign('priceSortText', 'Laag naar hoog');
          $this->pageTpl->assign('priceSortClass', 'sort-down');
          break;
        default:
          $this->pageTpl->assign('priceSortText', 'Laag naar hoog');
          $this->pageTpl->assign('priceSortClass', 'sort');
      }

      if ($sortPriceValue === 'noSort' && $sortAlfaValue === 'noSort') {
        //array_multisort(array_column($arrProducts, 'type'), SORT_ASC, $tempAlfa, SORT_ASC, $arrProducts);
	array_multisort(array_column($arrProducts, 'type'), SORT_ASC, ['Drank', 'Zuivel', 'Vlees', 'Charcuterie', 'Vis', 'Conserven', 'Snoepgoed', 'Wasproducten'], SORT_ASC, $arrProducts);
      }

      $this->pageTpl->assign('txtAlfabetSort', $sortAlfaValue);
      $this->pageTpl->assign('txtPriceSort', $sortPriceValue);
      $this->pageTpl->setIteration('iItems');

      for ($ix = 0; $ix < count($arrProducts); $ix++) {
        $imgUrl = 'core/img/thumb_' . $arrProducts[$ix]['id'] . '_4x3.jpg';

        if (!file_exists($imgUrl)) {
          $imgUrl = 'core/img/thumb_placeholder_4x3.jpg';
        }

        $arrOptions = HomeDB::getOptionsForProduct(intval($arrProducts[$ix]['id']));

        if ($arrOptions !== []) {
          $indexer = 0;
          $this->pageTpl->assignIterationOption('oHasOptions');
          $this->pageTpl->setIteration('iOptions', 'iItems');

          foreach ($arrOptions as $option) {
            if ($indexer > 0 && ($indexer + 1)%2 === 0) {
              $this->pageTpl->assignIterationOption('oNewOptionRow');
            }

            if ($option['type'] === 'Dropdown') {
              $this->pageTpl->assignIterationOption('oOptionIsDropdown');

              if (intval($option['isMandatory']) === 0) {
                $this->pageTpl->assignIterationOption('oIsNotMandatory');
                $this->pageTpl->assignIteration('defaultOptionText', $option['type_identifier'] . ' &ndash; Optioneel');
              }

              $this->pageTpl->setIteration('iOptionItems', 'iOptions');

              foreach ($option['rows'] as $optionRow) {
                $optionPrice = floatval($optionRow['price']);
                $optionText = $optionRow['name'];

                if ($optionPrice > floatval(0)) {
                  $optionText .= ' &ndash; &euro; ' . number_format($optionPrice, 2, '.', ' ');
                }

                $this->pageTpl->assignIteration('optionValue', $optionRow['id']);
                $this->pageTpl->assignIteration('optionText', $optionText);
                $this->pageTpl->refillIteration('iOptionItems');
              }

              $this->pageTpl->parseIteration('iOptionItems');
              $this->pageTpl->assignIteration('optionId', $option['type_quantifier']);
            }
            elseif ($option['type'] === 'Checkbox') {
              $this->pageTpl->assignIterationOption('oOptionIsCheckbox');
              $optionPrice = floatval($option['rows'][0]['price']);
              $optionText = $option['rows'][0]['name'];

              if ($optionPrice > floatval(0)) {
                $optionText .= ' &ndash; &euro; ' . number_format($optionPrice, 2, '.', ' ');
              }

              $this->pageTpl->assignIteration('optionId', $option['type_quantifier']);
              $this->pageTpl->assignIteration('optionText', $optionText);
              $this->pageTpl->assignIteration('optionValue', $option['rows'][0]['id']);
            }

            $this->pageTpl->refillIteration('iOptions');
            $indexer++;
          }

          $this->pageTpl->parseIteration('iOptions');
        }

        $this->pageTpl->assignIteration('itemTitle', $arrProducts[$ix]['name']);
        $this->pageTpl->assignIteration('imgAlt', $arrProducts[$ix]['name']);
        $this->pageTpl->assignIteration('imgUrl', $imgUrl);
        $this->pageTpl->assignIteration('price', number_format($arrProducts[$ix]['price'], 2, '.', ' '));
        $this->pageTpl->assignIteration('itemId', $arrProducts[$ix]['id']);
        $this->pageTpl->assignIteration('quantity', '1');

        if ($arrProducts[$ix]['quantifier'] !== NULL && $arrProducts[$ix]['quantifier'] !== '') {
          $this->pageTpl->assignIterationOption('oHasQuantifier');
          $this->pageTpl->assignIteration('itemQuantifier', $arrProducts[$ix]['quantifier']);
        }

        if ($arrProducts[$ix]['description'] !== NULL && $arrProducts[$ix]['description'] !== '') {
          $this->pageTpl->assignIterationOption('oHasDescription');
          $this->pageTpl->assignIteration('itemDescription', $arrProducts[$ix]['description']);
        }

        // Finally, set the iteration up for a next - you guessed it - iteration
        $this->pageTpl->refillIteration('iItems');
      }

      $this->pageTpl->parseIteration('iItems');
	}

	/**
	 * Doe acties op basis van de POST array
	 * @return
	 */
	public function doHome()
  {
    $this->view = 'home';
	}
}
?>
