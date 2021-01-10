<?php
/**
 * Checkout controller klasse
 * ------------------------------
 */

/**
 * ------------------------------
 * Includes
 * ------------------------------
 */
 require_once 'library/PHPMailer/PHPMailer.php';
 require_once 'library/PHPMailer/Exception.php';
 require_once 'library/PHPMailer/SMTP.php';
 use PHPMailer\PHPMailer\PHPMailer;
 use PHPMailer\PHPMailer\Exception;
 use PHPMailer\PHPMailer\SMTP;

  // ErrorDB
  require_once 'checkout.db.php';

class CheckoutController extends PlonkController
{
	// De verschillende views van de module
	protected $views = array(
    'checkout'
	);

	// De verschillende acties
	protected $actions = array(
    'checkout'
  );

	/**
	 * De view van de error pagina
	 * @return
	 */
	public function showCheckout()
  {
    // If there is no shopcart in the session, we have no business here, go back to get_browser
    if (!OneArchySession::exists('shopcartItems')) {
      PlonkWebsite::redirect($_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=home');
    }

    /**
     * Hoofdlayout
     */
  		// ken waardes toe aan hun bijhorende variabelen
		$this->mainTpl->assign('pageTitle', 'Promo Drinks - Klantgegevens');
		$this->mainTpl->assign('pageMeta',
      '<link rel="stylesheet" href="modules/checkout/css/checkouts.css" />
  <script src="modules/checkout/js/checkout.js"></script>');
    $this->pageTpl->assign('formUrl', $_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=checkout');
    $this->mainTpl->assign('shopcartUrl', $_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=shopcart');
    $arrFormContent = [
      'firstName' => '',
      'lastName' => '',
      'mail' => '',
      'phone' => '',
      'address' => '',
      'postal' => '',
      'city' => '',
      'addressExtra' => '',
      'postalExtra' => '',
      'cityExtra' => '',
      'multiAddress' => false
    ];
    $arrErrors = [];
    $arrArticles = [];

    // If we've gotten here, there is data in the session
    $arrArticles = OneArchySession::get('shopcartItems');

    if ($arrArticles === []) {
      PlonkWebsite::redirect($_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=home');
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

        if (isset($varArticle['id']) && intval($varArticle['id']) !== 0) {
          $shopcartTotal += (CheckoutDB::getProductPrice(intval($varArticle['id'])) * $tempQuantity);
        }

        if (isset($varArticle['options'])) {
          $extras = $varArticle['options'];

          if (count($extras) > 0) {
            foreach ($extras as $extra) {
              if (intval($extra['id']) > 0) {
                $shopcartTotal += floatval(CheckoutDB::getPriceForOption(intval($extra['id'])));
              }
            }
          }
        }
      }

      if ($shopcartTotal == 0 || $shopcartQuantity === 0) {
        PlonkWebsite::redirect($_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=home');
      }

      $this->mainTpl->assignOption('oShopcartVisible');
      $this->mainTpl->assign('shopcartQuantity', $shopcartQuantity);
      $this->mainTpl->assign('shopcartTotal', number_format($shopcartTotal, 2, '.', ' '));
      $this->mainTpl->assign('shopcartQuantitySmall', $shopcartQuantity);
      $this->mainTpl->assign('shopcartTotalSmall', number_format($shopcartTotal, 2, '.', ' '));
    }

    // Check the POST array for actions, we have either update and return to home, delete a certain item OR checkout
    if (isset($_POST['action'])) {
      if ($_POST['action'] === 'updateAndBack') {
        // We won't be needing errors, just go home
        $arrErrors = $this->updateClientInSession();
        PlonkWebsite::redirect($_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=shopcart');
      }
      elseif ($_POST['action'] === 'checkout') {
        // Update the shopping cart, send the order and go to the checkout page
        $arrErrors = $this->updateClientInSession();

        if ($arrErrors === []) {
          $orderInsert = $this->sendOrder();

          if ($orderInsert > 0) {
            PlonkWebsite::redirect($_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=thanks');
          }
        }
      }
      elseif (stripos($_POST['action'], 'continueBrowsing') !== false) {
        // We won't be needing errors, just go home
        $arrErrors = $this->updateClientInSession();
        PlonkWebsite::redirect($_SERVER['PHP_SELF'] . '?' . PlonkWebsite::$moduleKey . '=home');
      }
    }

    // Populate the form content if it was found in the session
    if (OneArchySession::exists('clientInfo')) {
      $arrFormContent = OneArchySession::get('clientInfo');
    }

    // Start setting the variables in the template
    $this->pageTpl->assign('txtFirstName', $arrFormContent['firstName']);
    $this->pageTpl->assign('txtLastName', $arrFormContent['lastName']);
    $this->pageTpl->assign('txtMail', $arrFormContent['mail']);
    $this->pageTpl->assign('txtPhone', $arrFormContent['phone']);
    $this->pageTpl->assign('txtAddress', $arrFormContent['address']);
    $this->pageTpl->assign('txtPostal', $arrFormContent['postal']);
    $this->pageTpl->assign('txtCity', $arrFormContent['city']);
    $this->pageTpl->assign('txtAddressExtra', $arrFormContent['addressExtra']);
    $this->pageTpl->assign('txtPostalExtra', $arrFormContent['postalExtra']);
    $this->pageTpl->assign('txtCityExtra', $arrFormContent['cityExtra']);

    if ($arrFormContent['multiAddress']) {
      $this->pageTpl->assignOption('oExtraChecked');
    }
    else {
      $this->pageTpl->assignOption('oExtraHidden');
    }

    if ($arrErrors === []) {
      $this->pageTpl->assign('formErrors', '');
      $this->pageTpl->assignOption('oHasNoErrors');
    }
    else {
      if (count($arrErrors) < 2 && stripos($arrErrors[0], 'mailadres of telefoonnummer in te vullen') !== false) {
        $this->pageTpl->assignOption('oHasErrors');
        $this->pageTpl->assign('formErrors', $arrErrors[0]);
      }
      else {
        $this->pageTpl->assignOption('oHasErrors');
        $this->pageTpl->assign('formErrors', 'Gelieve de verplichte velden in te vullen.');
        $this->pageTpl->setIteration('iErrorSpecifics');

        foreach ($arrErrors as $error) {
          if (stripos($error, 'mailadres of telefoonnummer in te vullen') !== false || stripos($error, 'in te vullen') === false) {
            $this->pageTpl->assignIteration('errorSpecific', $error);
            $this->pageTpl->refillIteration('iErrorSpecifics');
          }
        }

        $this->pageTpl->parseIteration();
      }
    }
	}

	/**
	 * Doe acties op basis van de POST array
	 * @return
	 */
	public function doCheckout()
  {
    // Do some stuff to put all updates into the session/cookie
    $this->view = 'checkout';
	}

  /**
   * Update all POST info into our session. Returns an arrar containing errors in case of missing data (empty otherwise).
   * @return array
   */
  private function updateClientInSession() {
    // Initialize the array that will end up in the session. Also returnvalue.
    $arrErrors = [];
    $arrFormContent = [
      'firstName' => '',
      'lastName' => '',
      'mail' => '',
      'phone' => '',
      'address' => '',
      'postal' => '',
      'city' => '',
      'addressExtra' => '',
      'postalExtra' => '',
      'cityExtra' => '',
      'multiAddress' => false
    ];

    // Go through all POST elements and look for the data to put in
    if (isset($_POST['txtFirstName']) && $_POST['txtFirstName'] !== '') {
      $arrFormContent['firstName'] = (string) $_POST['txtFirstName'];
    }
    else {
      $arrErrors[] = 'Gelieve je voornaam in te vullen';
      $this->pageTpl->assignOption('oFirstNameError');
    }

    if (isset($_POST['txtLastName']) && $_POST['txtLastName'] !== '') {
      $arrFormContent['lastName'] = (string) $_POST['txtLastName'];
    }
    else {
      $arrErrors[] = 'Gelieve je achternaam in te vullen';
      $this->pageTpl->assignOption('oLastNameError');
    }

    if (isset($_POST['txtMail']) && $_POST['txtMail'] !== '') {
      $arrFormContent['mail'] = (string) $_POST['txtMail'];

      if (!filter_var($arrFormContent['mail'], FILTER_VALIDATE_EMAIL)) {
        $arrErrors[] = 'Je mail adres is ongeldig';
        $this->pageTpl->assignOption('oMailError');
      }
    }
    else {
      $arrErrors[] = 'Gelieve je mail adres in te vullen';
      $this->pageTpl->assignOption('oMailError');
    }

    if (isset($_POST['txtPhone']) && $_POST['txtPhone'] !== '') {
      $arrFormContent['phone'] = (string) $_POST['txtPhone'];

      if (!preg_match('/^[0-9\-\(\)\/\+\s]*$/', $arrFormContent['phone'])) {
        $arrErrors[] = 'Je telefoonnummer is ongeldig';
        $this->pageTpl->assignOption('oPhoneError');
      }
    }
    else {
      $arrErrors[] = 'Gelieve je telefoonnummer in te vullen';
      $this->pageTpl->assignOption('oPhoneError');
    }

    if (isset($_POST['txtAddress']) && $_POST['txtAddress'] !== '') {
      $arrFormContent['address'] = (string) $_POST['txtAddress'];
      $splitAddress = explode(' ', $arrFormContent['address']);
      $properAddress = $splitAddress[0];

      for ($i = 1; $i < count($splitAddress); $i++) {
        if (strlen($splitAddress[$i]) > 0) {
          $properAddress .= ' ' . $splitAddress[$i];
        }
      }

      if (count(explode(' ', $properAddress)) < 2 || !preg_match('~[0-9]~', $properAddress)) {
        $arrErrors[] = 'Je adres is ongeldig';
        $this->pageTpl->assignOption('oAddressError');
      }
    }
    else {
      $arrErrors[] = 'Gelieve je adres in te vullen (straatnaam + huisnummer)';
      $this->pageTpl->assignOption('oAddressError');
    }

    if (isset($_POST['txtPostal']) && $_POST['txtPostal'] !== '') {
      $arrFormContent['postal'] = (string) $_POST['txtPostal'];

      if (!preg_match('/^([0-9]){4,5}$/', $arrFormContent['postal'])) {
        $arrErrors[] = 'Je postcode is ongeldig';
        $this->pageTpl->assignOption('oPostalError');
      }
    }
    else {
      $arrErrors[] = 'Gelieve je postcode in te vullen';
      $this->pageTpl->assignOption('oPostalError');
    }

    if (isset($_POST['txtCity']) && $_POST['txtCity'] !== '') {
      $arrFormContent['city'] = (string) $_POST['txtCity'];
    }
    else {
      $arrErrors[] = 'Gelieve je woonplaats in te vullen';
      $this->pageTpl->assignOption('oCityError');
    }

    if (isset($_POST['chkMultiAddress'])) {
      if (isset($_POST['txtAddressExtra']) && $_POST['txtAddressExtra'] !== '') {
        $arrFormContent['addressExtra'] = (string) $_POST['txtAddressExtra'];
        $splitAddress = explode(' ', $arrFormContent['addressExtra']);
        $properAddress = $splitAddress[0];

        for ($i = 1; $i < count($splitAddress); $i++) {
          if (strlen($splitAddress[$i]) > 0) {
            $properAddress .= ' ' . $splitAddress[$i];
          }
        }

        if (count(explode(' ', $properAddress)) < 2 || !preg_match('~[0-9]~', $properAddress)) {
          $arrErrors[] = 'Je facturatie adres is ongeldig';
          $this->pageTpl->assignOption('oAddressExtraError');
        }
      }

      if (isset($_POST['txtPostalExtra']) && $_POST['txtPostalExtra'] !== '') {
        $arrFormContent['postalExtra'] = (string) $_POST['txtPostalExtra'];

        if (!preg_match('/^([0-9]){4,5}$/', $arrFormContent['postalExtra'])) {
          $arrErrors[] = 'Je facturatie postcode is ongeldig';
          $this->pageTpl->assignOption('oPostalExtraError');
        }
      }

      if (isset($_POST['txtCityExtra']) && $_POST['txtCityExtra'] !== '') {
        $arrFormContent['cityExtra'] = (string) $_POST['txtCityExtra'];
      }
    }

    if (isset($_POST['chkMultiAddress'])) {
      $arrFormContent['multiAddress'] = true;
    }

    // Finally, put all data inside the session
    OneArchySession::set('clientInfo', $arrFormContent);

    // All required fields are filled in
    return $arrErrors;
  }

  /**
   * Process all POST and session data and send the order
   * Return -1 if nothing was done, 0 if failed and 1 if succesful.
   * @return int
   */
  private function sendOrder() {
    if (OneArchySession::exists('shopcartItems') && OneArchySession::exists('clientInfo')) {
      $shopcartItems = (array) OneArchySession::get('shopcartItems');
      $clientInfo = (array) OneArchySession::get('clientInfo');
      $comment = '';

      if (OneArchySession::exists('shopcartComment')) {
        $comment = OneArchySession::get('shopcartComment');
      }

      $orderId = (int) CheckoutDB::insertOrder($clientInfo, $shopcartItems, $comment);

      if ($this->mailOrder($orderId) < 1) {
        return -1;
      }

      return $orderId;
    }

    return -1;
  }

  /**
   * Sends mails to both client and seller
   * Return -1 if nothing was done, 0 if failed and 1 if succesful.
   * @return int
   */
  private function mailOrder($orderId) {
    $orderId = (int) $orderId;
    $clientTable = '';
    $otherClientTable = '';
    $checkoutTotal = 0;
    $arrArticles = [];
    $clientInfo = [];
    $comment = 'Geen';

    if (OneArchySession::exists('shopcartComment')) {
      $comment = OneArchySession::get('shopcartComment');

      if ($comment === '') {
        $comment = 'Geen';
      }
    }

    if ($orderId < 1) {
      return -1;
    }

    $clientInfo = CheckoutDB::getClientInfoFromOrder($orderId);

    if ($clientInfo === []) {
      return -1;
    }

    $clientTable = '<table cellspacing="0" style="width:50%;margin:20px 0;background-color:#FAFAFA;padding:20px;">
      <tr>
        <td><strong>Naam: </strong></td>
        <td>' . $clientInfo['firstName'] . ' ' . $clientInfo['lastName'] . '</td>
      </tr>
      <tr>
        <td><strong>Telnr: </strong></td>
        <td>' . $clientInfo['phoneNumber'] . '</td>
      </tr>
      <tr>
        <td><strong>E-mail: </strong></td>
        <td>' . $clientInfo['email'] . '</td>
      </tr>
      <tr>
        <td><strong>Opmerking: </strong></td>
        <td>' . $comment . '</td>
      </tr>
    </table>';

    if ($clientInfo['addressLine2'] !== NULL && $clientInfo['addressLine2'] !== '') {
      $otherClientTable = '<table cellspacing="0" style="width:100%;margin-top:20px;background-color:#FAFAFA;padding:20px;">
        <tr>
          <th>Leveradres</th>
          <th>&nbsp;</th>
          <th style="border-left: 1px solid #CCC;">Factuuradres</th>
          <th>&nbsp;</th>
        </tr>
        <tr>
          <td style="padding-top:10px;"><strong>Adres: </strong></td>
          <td style="padding-top:10px;">' . $clientInfo['addressLine1'] . '</td>
          <td style="border-left: 1px solid #CCC;padding-top:10px;"><strong>Adres: </strong></td>
          <td style="padding-top:10px;">' . $clientInfo['addressLine2'] . '</td>
        </tr>
        <tr>
          <td><strong>Postcode + woonplaats: </strong></td>
          <td>' . $clientInfo['postal1'] . ' ' . $clientInfo['city1'] . '</td>
          <td style="border-left: 1px solid #CCC;"><strong>Postcode + woonplaats: </strong></td>
          <td>' . $clientInfo['postal2'] . ' ' . $clientInfo['city2'] . '</td>
        </tr>
      </table>';
    }
    else {
      $otherClientTable = '<table cellspacing="0" style="width:50%;margin-top:20px;background-color:#FAFAFA;padding:20px;">
        <tr>
          <th style="border-bottom:1px solid #CCC;">Leveradres</th>
          <th style="border-bottom:1px solid #CCC;">&nbsp;</th>
        </tr>
        <tr>
          <td style="padding-top:10px;"><strong>Adres: </strong></td>
          <td style="padding-top:10px;">' . $clientInfo['addressLine1'] . '</td>
        </tr>
        <tr>
          <td><strong>Postcode + woonplaats: </strong></td>
          <td>' . $clientInfo['postal1'] . ' ' . $clientInfo['city1'] . '</td>
        </tr>
      </table>';
    }

    $htmlSellerHeader = '<body style="min-height:100vh;display:flex;flex-direction:column;box-sizing:border-box;;background-color: #f5edb3;display: flex;flex-direction:column;font-family:-apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,Oxygen,Ubuntu,Cantarell,"Fira Sans","Droid Sans","Helvetica Neue",Helvetica,Arial,sans-serif;margin:0;padding:0;line-height:1.5;color:#4a4a4a;font-weight:400;-webkit-font-smoothing:antialiased;text-rendering:optimizeLegibility;min-width:300px;overflow-x:hidden;overflow-y:scroll;">
    <div style="position:relative;box-sizing:inherit;display:block;">
      <h2 style="font-size:2rem;margin-bottom:1.5rem;font-weight:300;line-height:1.125;word-break:break-word;margin:0;padding:0;box-sizing:inherit;display:block;margin-block-start:0.83rem;margin-block-end:0.83rem;margin-inline-start:0px;margin-inline-end:0px;">
        Nieuwe bestelling #' . $orderId . ' bij Promo Drinks!
      </h2>
      <h3 style="font-size:1.2rem;margin-top:-1.25rem;color:#4a4a4a;font-weight:300;line-height:1.25;word-break:break-word;margin:0;padding:0;margin-bottom:1.5rem;box-sizing:inherit;display:block;margin-block-start:0.83rem;margin-block-end:0.83rem;margin-inline-start:0px;margin-inline-end:0px;">
        Bestelinfo:
      </h3>';

    $htmlClientHeader = '<body style="min-height:100vh;display:flex;flex-direction:column;box-sizing:border-box;;background-color: #f5edb3;display: flex;flex-direction:column;font-family:-apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,Oxygen,Ubuntu,Cantarell,"Fira Sans","Droid Sans","Helvetica Neue",Helvetica,Arial,sans-serif;margin:0;padding:0;line-height:1.5;color:#4a4a4a;font-weight:400;-webkit-font-smoothing:antialiased;text-rendering:optimizeLegibility;min-width:300px;overflow-x:hidden;overflow-y:scroll;">
    <div style="position:relative;box-sizing:inherit;display:block;">
      <h2 style="font-size:2rem;margin-bottom:1.5rem;font-weight:300;line-height:1.125;word-break:break-word;margin:0;padding:0;box-sizing:inherit;display:block;margin-block-start:0.83rem;margin-block-end:0.83rem;margin-inline-start:0px;margin-inline-end:0px;">
        Bedankt voor uw bestelling #' . $orderId . ' bij Promo Drinks!
      </h2>
      <h3 style="font-size:1.2rem;margin-top:-1.25rem;color:#4a4a4a;font-weight:300;line-height:1.25;word-break:break-word;margin:0;padding:0;margin-bottom:1.5rem;box-sizing:inherit;display:block;margin-block-start:0.83rem;margin-block-end:0.83rem;margin-inline-start:0px;margin-inline-end:0px;">
        Bestelinfo:
      </h3>';

    $htmlTable = '<table cellspacing="0" style="width:100%;background-color:#FAFAFA;padding:10px;">
        <thead>
          <tr style="text-align: center;">
            <th style="width:50%;border-bottom:1px solid #999;">Info</th>
            <th style="width:20%;border-bottom:1px solid #999;">Prijs</th>
            <th style="width:15%;border-bottom:1px solid #999;">Aantal</th>
            <th style="width:15%;border-bottom:1px solid #999;">Totaal</th>
          </tr>
        </thead>
        <tbody>';

    if (OneArchySession::exists('shopcartItems')) {
      $arrArticles = OneArchySession::get('shopcartItems');

      if ($arrArticles !== []) {
      	foreach ($arrArticles as $article) {
          $article['info'] = CheckoutDB::getProductInfoById($article['id']);

    			if (isset($article['options']) && count($article['options']) > 0) {
            $article['optionInfos'] = CheckoutDB::getOptionInfoByIds(array_column($article['options'], 'id'));
      		}

          $quantity = intval($article['quantity']);
          $subPrice = floatval($article['info']['price']);
          $extraPrice = 0.0;
          $htmlTable .= '<tr>
              <td style="width:50%;border-bottom:1px solid #CCC;">
                <h4>' . $article['info']['name'] . '</h4>';

    			if (isset($article['optionInfos']) && $article['optionInfos'] !== []) {
    				$htmlTable .= '<p>Met ';

            for($i = 0; $i < count($article['optionInfos']); $i++) {
              $htmlTable .= strtolower($article['optionInfos'][$i]['type_identifier'] . ' = ' . $article['optionInfos'][$i]['name']);

              if (floatval($article['optionInfos']) > 0.0) {
                $extraPrice += floatval($article['optionInfos'][$i]['price']);
              }

              if ($i !== (count($article['optionInfos']) - 1)) {
                $htmlTable .= ', ';
              }
            }

            $htmlTable .= '</p>';
    			}

          $htmlTable .= '</td>
              <td style="width:20%;text-align: center;border-bottom:1px solid #CCC;">&euro; ' . number_format($subPrice, 2, '.', ' ');

    			if ($extraPrice > 0.0) {
    				$htmlTable .= '<br />+ ' . number_format($extraPrice, 2, '.', ' ');
    				$subPrice += $extraPrice;
    			}

          $subTotal = floatval($subPrice * $quantity);
          $htmlTable .= '</td>
              <td style="width:15%;text-align: center;border-bottom:1px solid #CCC;">' . intval($article['quantity']) . '</td>
              <td style="width:15%;text-align: center;border-bottom:1px solid #CCC;">&euro; ' . number_format($subTotal, 2, '.', ' ') . '</td>
            </tr>';

          $checkoutTotal += $subTotal;
        }


        $htmlTable .= '</tbody>
            <tfoot>
              <tr style="text-align: center;">
                <th style="width:50%;border-top:1px solid #999;">&nbsp;</th>
                <th style="width:20%;border-top:1px solid #999;">&nbsp;</th>
                <th style="width:15%;border-top:1px solid #999;">Totaal: </th>
                <th style="width:15%;border-top:1px solid #999;">&euro; ' . number_format($checkoutTotal, 2, '.', ' ') . '</th>
              </tr>
            </tfoot>
          </table>';

        $recipient = $clientInfo['email'];
        $subject = 'Uw bestelling bij Promo Drinks';
        $headers = 'Content-type:text/html;From:' . PD_MAIL_SENDER . ';';
        $mail = new PHPMailer(true);
        $mail->From = PD_MAIL_SENDER;
        $mail->FromName = 'Promo Drinks';
        $mail->addBCC(PD_MAIL_SENDER, 'Promo Drinks');
        $mail->addAddress($recipient);
        $mail->isHTML(true);
        $mail->Subject = $subject;
        $mail->Body = $htmlClientHeader . $clientTable . $htmlTable . $otherClientTable . '</div>';
        $mail->isSMTP();
        $mail->Host = PD_MAIL_HOST;
        $mail->Port = PD_MAIL_PORT;
        $mail->SMTPAuth = true;
        $mail->Username = PD_MAIL_SENDER;
        $mail->Password = PD_MAIL_PASS;

        try {
          $mail->send();
          //echo $htmlClientHeader . $clientTable . $htmlTable . $otherClientTable . '</div>';
        } catch (Exception $ex) {
          echo $ex->getMessage();
          // Let's wait out and see if the seller gets it
        }

        $subject = 'Nieuwe bestelling bij Promo Drinks';
        $headers = 'Content-type:text/html;From:' . PD_MAIL_SENDER . ';';
        $mail = new PHPMailer(true);
        $mail->From = PD_MAIL_SENDER;
        $mail->FromName = 'Promo Drinks';
        $mail->addAddress(PD_MAIL_SELLER);
        $mail->addBCC(PD_MAIL_SENDER, 'Promo Drinks');
        $mail->isHTML(true);
        $mail->Subject = $subject;
        $mail->Body = $htmlSellerHeader . $clientTable . $htmlTable . $otherClientTable . '</div>';
        $mail->isSMTP();
        $mail->Host = PD_MAIL_HOST;
        $mail->Port = PD_MAIL_PORT;
        $mail->SMTPAuth = true;
        $mail->Username = PD_MAIL_SENDER;
        $mail->Password = PD_MAIL_PASS;

        try {
          $mail->send();
        } catch (Exception $ex) {
          echo $ex->getMessage();
          return -1;
        }
      }
    }

    return 1;
  }
}
?>
