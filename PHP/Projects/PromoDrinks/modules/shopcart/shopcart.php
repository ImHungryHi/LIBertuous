<?php
/**
 * Shopcart controller klasse
 * ------------------------------
 */

/**
 * ------------------------------
 * Includes
 * ------------------------------
 */

  // ShopcartDB
  require_once 'shopcart.db.php';

class ShopcartController extends PlonkController
{
	// De verschillende views van de module
	protected $views = array(
    'shopcart'
	);

	// De verschillende acties
	protected $actions = array(
    'shopcart'
  );

	/**
	 * De view van de shopcart pagina
	 * @return
	 */
	public function showShopcart()
  {
    /**
     * Hoofdlayout
     */
  		// ken waardes toe aan hun bijhorende variabelen
  		$this->mainTpl->assign('pageTitle', 'Promo Drinks - Winkelwagen');
  		$this->mainTpl->assign('pageMeta',
      '<link rel="stylesheet" href="modules/shopcart/css/shopcart.css" />
    <script src="modules/shopcart/js/shopcart.js"></script>');
      $this->pageTpl->assign('formUrl', $_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=shopcart');
      $this->mainTpl->assign('shopcartUrl', $_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=shopcart');
      $arrArticles = array();

      // Check the POST array for actions, we have either update and return to home, delete a certain item OR checkout
      if (isset($_POST['action'])) {
        if ($_POST['action'] === 'updateAndBack') {
          // Update the shopping cart and go back to the homepage
          $this->updateShopcartInSession();
          PlonkWebsite::redirect($_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=home');
        }
        elseif ($_POST['action'] === 'checkout') {
          // Update the shopping cart, send the order and go to the checkout page
          $this->updateShopcartInSession();
          PlonkWebsite::redirect($_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=checkout');
        }
        elseif (stripos($_POST['action'], 'btnDeleteFor_') !== false) {
          $this->updateShopcartInSession();
        }
      }

      // Check if there is a comment in the session and update the comment field if there is
      if (OneArchySession::exists('shopcartComment')) {
        $this->pageTpl->assign('txtCommentContent', (string) OneArchySession::get('shopcartComment'));
      }
      else {
        $this->pageTpl->assign('txtCommentContent', '');
      }

      try {
        $arrArticles = OneArchySession::get('shopcartItems');
      }
      catch(Exception $e) {
        // If an exception was thrown here, we can expect that there is no entry in the session for shopcartItems. Just move on...
      }

      // Iteratie van winkelwagen artikelen
      if (gettype($arrArticles) === 'array' && count($arrArticles) > 0) {
        $arrArticleIds = array_column($arrArticles, 'id');
        array_multisort($arrArticleIds, SORT_ASC, $arrArticles);
        $shopcartQuantity = 0;
        $total = 0;
        $arrArticleInfos = array();

        $arrArticleInfos = ShopcartDB::getProductInfoByIds($arrArticleIds);
        $arrArticleOptions = ShopcartDB::getProductOptions($arrArticleIds);

        // If the array was tempered with in any way to contain non-existing articles, we won't show anything
        if ($arrArticleInfos === NULL || gettype($arrArticleInfos) !== 'array' || $arrArticleInfos === []) {
          $this->pageTpl->assignOption('oHasNoItems');
          $this->mainTpl->assignOption('oShopcartHidden');
          $this->mainTpl->assign('shopcartQuantity', '0');
          $this->mainTpl->assign('shopcartTotal', '0');
          $this->mainTpl->assign('shopcartQuantitySmall', '0');
          $this->mainTpl->assign('shopcartTotalSmall', '0');
        }
        else {
          for ($x = 0; $x < count($arrArticles); $x++) {
            foreach ($arrArticleInfos as $articleInfos) {
              if ($arrArticles[$x]['id'] === $articleInfos['id']) {
                $arrArticles[$x]['info'] = $articleInfos;
              }
            }

            foreach ($arrArticleOptions as $articleOption) {
              if ($arrArticles[$x]['id'] === $articleOption['productId']) {
                $arrArticles[$x]['optionData'][$articleOption['type_quantifier']][] = $articleOption;
              }
            }
          }

          $this->pageTpl->assignOption('oHasItems');
          $this->pageTpl->setIteration('iItems');

          for ($x = 0; $x < count($arrArticles); $x++) {
            $subTotal = 0;
            $itemPrice = 0;

            if (intval($arrArticles[$x]['quantity']) > 0) {
              if ($arrArticles[$x]['info']['description'] !== NULL && $arrArticles[$x]['info']['description'] !== '') {
                $this->pageTpl->assignIterationOption('oHasDescription');
                $this->pageTpl->assignIteration('itemDescription', $arrArticles[$x]['info']['description']);
              }

              if ($arrArticles[$x]['info']['quantifier'] !== NULL && $arrArticles[$x]['info']['quantifier'] !== '') {
                $this->pageTpl->assignIterationOption('oHasQuantifier');
                $this->pageTpl->assignIteration('itemQuantifier', $arrArticles[$x]['info']['quantifier']);
              }

              $itemPrice = floatval($arrArticles[$x]['info']['price']);

              if (isset($arrArticles[$x]['optionData']) && count($arrArticles[$x]['optionData']) > 0) {
                $this->pageTpl->assignIterationOption('oHasOptions');
                $this->pageTpl->setIteration('iOptions', 'iItems');

                foreach ($arrArticles[$x]['optionData'] as $articleOption) {
                  if ($articleOption[0]['type'] === 'Checkbox') {
                    $this->pageTpl->assignIterationOption('oOptionIsCheckbox');
                    $this->pageTpl->assignIteration('optionValue', $articleOption[0]['optionId']);
                    $this->pageTpl->assignIteration('optionText', $articleOption[0]['optionName']);

                    if (floatval($articleOption[0]['optionPrice']) > 0.0) {
                      $this->pageTpl->assignIterationOption('oOptionHasCost');
                      $this->pageTpl->assignIteration('optionPrice', number_format(floatval($articleOption[0]['optionPrice']), 2, '.', ' '));
                    }

                    // Loop through the given options (= added to the article in the cart). If not chosen, it is not in the array
                    foreach ($arrArticles[$x]['options'] as $givenOption) {
                      if ($givenOption['quantifier'] === $articleOption[0]['type_quantifier']) {
                        $this->pageTpl->assignIterationOption('oOptionChecked');
                        $itemPrice += floatval($articleOption[0]['optionPrice']);
                      }
                    }
                  }
                  else {
                    $this->pageTpl->assignIterationOption('oOptionIsDropdown');

                    if (intval($articleOption[0]['isMandatory']) !== 1) {
                      $this->pageTpl->assignIterationOption('oIsNotMandatory');
                      $this->pageTpl->assignIteration('defaultOptionText', $articleOption[0]['type_identifier'] . ' &ndash; Optioneel');
                    }

                    $this->pageTpl->setIteration('iOptionItems', 'iOptions');

                    foreach ($articleOption as $optionItem) {
                      if (floatval($optionItem['optionPrice']) > 0.0) {
                        $this->pageTpl->assignIterationOption('oOptionHasPrice');
                        $this->pageTpl->assignIteration('optionPrice', number_format(floatval($optionItem['optionPrice']), 2, '.', ' '));
                      }

                      $this->pageTpl->assignIteration('optionValue', $optionItem['optionId']);
                      $this->pageTpl->assignIteration('optionText', $optionItem['optionName']);

                      // Loop through the given options (= added to the article in the cart). If not chosen, it is not in the array
                      foreach ($arrArticles[$x]['options'] as $givenOption) {
                        if ($givenOption['quantifier'] === $optionItem['type_quantifier'] && $givenOption['id'] === $optionItem['optionId']) {
                          $this->pageTpl->assignIterationOption('oOptionSelected');
                          $itemPrice += floatval($articleOption[0]['optionPrice']);
                        }
                      }

                      $this->pageTpl->refillIteration('iOptionItems');
                    }

                    $this->pageTpl->parseIteration('iOptionItems');
                  }

                  $this->pageTpl->assignIteration('optionId', $articleOption[0]['type_quantifier']);
                  $this->pageTpl->refillIteration('iOptions');
                }

                $this->pageTpl->parseIteration('iOptions');
              }

              if (intval($arrArticles[$x]['quantity']) > 1000) {
                $arrArticles[$x]['quantity'] = 1000;
              }

              $this->pageTpl->assignIteration('itemId', $arrArticles[$x]['id'] . 'x' . $x);
              $itemPrice *= floatval($arrArticles[$x]['quantity']);
              $this->pageTpl->assignIteration('artTitle', $arrArticles[$x]['info']['name']);
              $this->pageTpl->assignIteration('artPrice', number_format(floatval($arrArticles[$x]['info']['price']), 2, '.', ' '));
              $this->pageTpl->assignIteration('artQuantity', $arrArticles[$x]['quantity']);
              $this->pageTpl->assignIteration('artTotal', number_format($itemPrice, 2, '.', ' '));
              $shopcartQuantity += intval($arrArticles[$x]['quantity']);
              $total += $itemPrice;
              $this->pageTpl->refillIteration('iItems');
            }
          }

          $this->pageTpl->parseIteration();
          $this->pageTpl->assign('shopcartTotal', number_format($total, 2, '.', ' '));
          $this->mainTpl->assignOption('oShopcartVisible');
          $this->mainTpl->assign('shopcartQuantity', $shopcartQuantity);
          $this->mainTpl->assign('shopcartTotal', number_format($total, 2, '.', ' '));
          $this->mainTpl->assign('shopcartQuantitySmall', $shopcartQuantity);
          $this->mainTpl->assign('shopcartTotalSmall', number_format($total, 2, '.', ' '));
        }
      }
      else {
        $this->pageTpl->assignOption('oHasNoItems');
        $this->mainTpl->assignOption('oShopcartHidden');
        $this->mainTpl->assign('shopcartQuantity', '0');
        $this->mainTpl->assign('shopcartTotal', '0');
        $this->mainTpl->assign('shopcartQuantitySmall', '0');
        $this->mainTpl->assign('shopcartTotalSmall', '0');
      }
	}

  /**
   * Update via POST
   * @return
   */
  public function doShopcart() {
    // Do some stuff to put all updates into the session/cookie
    $this->view = 'shopcart';
  }

  /**
   * Update all POST info into our shopcart
   * @return
   */
  private function updateShopcartInSession() {
    $arrArticles = array();
    $arrLinkedOptions = array();
    $deleteForId = '';

    if (isset($_POST['action']) && stripos($_POST['action'], 'btnDeleteFor_') !== false) {
      $deleteForId = explode('_', $_POST['action'])[1];
    }

    // Parse the POST array into something that resembles the session array a little more
    foreach ($_POST as $k => $v) {
      $arrSplit = stripos($k, '_') !== false ? explode('_', $k) : $k;

      if ($arrSplit !== $k && count($arrSplit) > 1 && $deleteForId !== $arrSplit[1]) {
        $arrSplitIdKey = explode('x', $arrSplit[1]);
        $intItemId = intval($arrSplitIdKey[1]);
        $arrLinkedOptions[$intItemId]['id'] = intval($arrSplitIdKey[0]);
        $arrLinkedOptions[$intItemId]['quantity'] = 0;
        $blnFoundIt = false;

        if (stripos($k, 'selQuantityFor_') !== false && $v !== null && $v !== '') {
          $arrLinkedOptions[$intItemId]['quantity'] = intval($_POST['selQuantityFor_' . $arrSplit[1]]);

          if ($arrLinkedOptions[$intItemId]['quantity'] > 1000) {
            $arrLinkedOptions[$intItemId]['quantity'] = 1000;
          }
        }
        elseif (stripos($k, 'chkOption') !== false && $v !== null && $v !== '') {
          // Get quantifier before underscore and productIdxinstanceId after
          $arrLinkedOptions[$intItemId]['options'][] = ['quantifier' => intval(str_replace('chkOption', '', $arrSplit[0])), 'id' => intval($v)];
        }
        elseif (stripos($k, 'selOption') !== false && $v !== null && $v !== '') {
          // Get quantifier before underscore and productIdxinstanceId after
          $arrLinkedOptions[$intItemId]['options'][] = ['quantifier' => intval(str_replace('selOption', '', $arrSplit[0])), 'id' => intval($v)];
        }

        if (count($arrArticles) === 0 && $arrLinkedOptions[$intItemId]['quantity'] > 0) {
          $arrArticles[] = $arrLinkedOptions[$intItemId];
        }
        elseif ($arrLinkedOptions[$intItemId]['quantity'] > 0) {
          for ($x = 0; $x < count($arrArticles); $x++) {
            if ($arrArticles[$x]['id'] === $arrLinkedOptions[$intItemId]['id'] && $arrArticles[$x]['options'] === $arrLinkedOptions[$intItemId]['options']) {
              $arrArticles[$x]['quantity'] += $arrLinkedOptions[$intItemId]['quantity'];
              $blnFoundIt = true;

              if ($arrArticles[$x]['quantity'] > 1000) {
                $arrArticles[$x]['quantity'] = 1000;
              }

              break;
            }
          }

          if (!$blnFoundIt) {
            $arrArticles[] = $arrLinkedOptions[$intItemId];
          }
        }
      }
    }

    // Pass all articles through to the session in their altered states
    OneArchySession::set('shopcartItems', $arrArticles);

    if (isset($_POST['txtComment'])) {
      OneArchySession::set('shopcartComment', (string) $_POST['txtComment']);
    }
  }
}
?>
