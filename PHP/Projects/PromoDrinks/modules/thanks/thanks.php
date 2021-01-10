<?php
/**
 * Thanks controller klasse
 * ------------------------------
 */

/**
 * ------------------------------
 * Includes
 * ------------------------------
 */

  // ThanksDB
  require_once 'thanks.db.php';

class ThanksController extends PlonkController
{
	// De verschillende views van de module
	protected $views = array(
    'thanks'
	);

	// De verschillende acties
	protected $actions = array();

	/**
	 * De view van de thanks pagina
	 * @return
	 */
	public function showThanks()
  {
    /**
     * Hoofdlayout
     */
  		// ken waardes toe aan hun bijhorende variabelen
  		$this->mainTpl->assign('pageTitle', 'Promo Drinks - Afrekening');
  		$this->mainTpl->assign('pageMeta',
      '<link type="text/css" rel="stylesheet" href="modules/thanks/css/thanks.css" />');
      $this->pageTpl->assign('browseLink', $_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=home');
      $this->mainTpl->assign('shopcartUrl', $_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=shopcart');
      $this->mainTpl->assignOption('oShopcartHidden');
      $this->mainTpl->assign('shopcartQuantity', '0');
      $this->mainTpl->assign('shopcartTotal', '0');
      $this->mainTpl->assign('shopcartQuantitySmall', '0');
      $this->mainTpl->assign('shopcartTotalSmall', '0');

      // Iteratie van winkelwagen artikelen
      if (!OneArchySession::exists('shopcartItems')) {
        PlonkWebsite::redirect($_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=home');
      }
      else {
        // Array nog toevoegen van op te zoeken artikelen
        $arrArticles = OneArchySession::get('shopcartItems');
        $arrArticleIds = array_column($arrArticles, 'id');
        array_multisort($arrArticleIds, SORT_ASC, $arrArticles);
        $total = 0;
        $this->pageTpl->setIteration('iItems');

        for ($x = 0; $x < count($arrArticles); $x++) {
          $arrArticles[$x]['info'] = ThanksDB::getProductInfoById($arrArticles[$x]['id']);
          $subTotal = 0.0;
          $extraPrice = 0.0;

          if (isset($arrArticles[$x]['options']) && count($arrArticles[$x]['options']) > 0) {
            $arrArticles[$x]['optionInfos'] = ThanksDB::getOptionInfoByIds(array_column($arrArticles[$x]['options'], 'id'));

            if ($arrArticles[$x]['info']['description'] !== '' && $arrArticles[$x]['info']['quantifier'] !== '') {
              $this->pageTpl->assignIterationOption('oDescription');
              $this->pageTpl->assignIteration('descriptionContent', $arrArticles[$x]['info']['description'] . ', ' . $arrArticles[$x]['info']['quantifier']);
            }
            elseif ($arrArticles[$x]['info']['description'] !== '') {
              $this->pageTpl->assignIterationOption('oDescription');
              $this->pageTpl->assignIteration('descriptionContent', $arrArticles[$x]['info']['description']);
            }
            elseif ($arrArticles[$x]['info']['quantifier'] !== '') {
              $this->pageTpl->assignIterationOption('oDescription');
              $this->pageTpl->assignIteration('descriptionContent', $arrArticles[$x]['info']['quantifier']);
            }

            if (count($arrArticles[$x]['optionInfos']) > 0) {
              $this->pageTpl->assignIterationOption('oHasExtras');
              $this->pageTpl->setIteration('iOptions', 'iItems');

              foreach ($arrArticles[$x]['optionInfos'] as $option) {
                $optionContent = $option['type_identifier'] . ' = ' . $option['name'];

                if (floatval($option['price']) > 0.0) {
                  $optionContent .= '&nbsp;(+&nbsp;&euro;&ndash;' . number_format($option['price'], 2, '.', ' ') . ')';
                  $extraPrice += floatval($option['price']);
                }

                if ($option !== $arrArticles[$x]['optionInfos'][count($arrArticles[$x]['optionInfos']) - 1]) {
                  $optionContent .= ', ';
                }

                $this->pageTpl->assignIteration('optionContent', $optionContent);
                $this->pageTpl->refillIteration('iOptions');
              }

              $this->pageTpl->parseIteration('iOptions');
            }
          }

          $subTotal = floatval($arrArticles[$x]['quantity'] * (floatval($arrArticles[$x]['info']['price']) + floatval($extraPrice)));
          $this->pageTpl->assignIteration('artTitle', $arrArticles[$x]['info']['name']);
          $this->pageTpl->assignIteration('artPrice', $arrArticles[$x]['info']['price']);
          $this->pageTpl->assignIteration('artQuantity', $arrArticles[$x]['quantity']);
          $this->pageTpl->assignIteration('artTotal', number_format($subTotal, 2, '.', ' '));
          $total += $subTotal;
          $this->pageTpl->refillIteration();
        }

        $this->pageTpl->parseIteration();
        $this->pageTpl->assign('checkoutTotal', number_format((float) $total, 2, '.', ''));

        // Destroy the session and its data, we'll not be needing it anymore
        OneArchySession::destroy();
      }
	}
}
?>
