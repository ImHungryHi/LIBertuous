<?php
/**
 * Error controller klasse
 * ------------------------------
 */

/**
 * ------------------------------
 * Includes
 * ------------------------------
 */

  // ErrorDB
  require_once 'error.db.php';

class ErrorController extends PlonkController
{
	// De verschillende views van de module
	protected $views = array(
    'error'
	);

	// De verschillende acties
	protected $actions = array();

	/**
	 * De view van de error pagina
	 * @return
	 */
	public function showError()
  {
    /**
     * Hoofdlayout
     */
  		// ken waardes toe aan hun bijhorende variabelen
  		$this->mainTpl->assign('pageTitle', 'Promo Drinks - Foutpagina');
  		$this->mainTpl->assign('pageMeta', '<link type="text/css" rel="stylesheet" href="modules/error/css/error.css" />');
      $this->mainTpl->assign('shopcartUrl', $_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=shopcart');
      $this->pageTpl->assign('browseUrl', $_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=home');
      $arrArticles = [];

      if (OneArchySession::exists('shopcartItems')) {
        $arrArticles = OneArchySession::get('shopcartItems');
      }

      if ($arrArticles === []) {
        $this->mainTpl->assignOption('oShopcartHidden');
        $this->mainTpl->assign('shopcartQuantity', '0');
        $this->mainTpl->assign('shopcartTotal', '0');
        $this->mainTpl->assign('shopcartQuantitySmall', '0');
        $this->mainTpl->assign('shopcartTotalSmall', '0');
      }
      else {
        $shopcartQuantity = 0;
        $shopcartTotal = 0;

        foreach ($arrArticles as $varArticle) {
          $tempQuantity = 0;

          if (isset($varArticle['quantity'])) {
            $tempQuantity = intval($varArticle['quantity']);
            $shopcartQuantity += $tempQuantity;
          }

          if (isset($varArticle['options']) && count($varArticle['options']) > 0) {
            foreach ($varArticle['options'] as $option) {
              if (intval($option['id']) > 0) {
                $shopcartTotal += (ErrorDB::getOptionPrice(intval($option['id'])) * $tempQuantity);
              }
            }
          }

          if (isset($varArticle['id']) && intval($varArticle['id']) !== 0) {
            $shopcartTotal += (ErrorDB::getProductPrice(intval($varArticle['id'])) * $tempQuantity);
          }
        }

        $this->mainTpl->assignOption('oShopcartVisible');
        $this->mainTpl->assign('shopcartQuantity', $shopcartQuantity);
        $this->mainTpl->assign('shopcartTotal', number_format($shopcartTotal, 2, '.', ' '));
        $this->mainTpl->assign('shopcartQuantitySmall', $shopcartQuantity);
        $this->mainTpl->assign('shopcartTotalSmall', number_format($shopcartTotal, 2, '.', ' '));
      }

      if (OneArchySession::exists('errorMessage') && defined('DEBUG') && (DEBUG === true)) {
        $this->pageTpl->assignOption('oHasDebugging');
        $this->pageTpl->assign('errorMessage', OneArchySession::get('errorMessage'));
      }

      OneArchySession::remove('errorMessage');
	}
}
?>
