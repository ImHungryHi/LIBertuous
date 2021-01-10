<?php
/**
 * ------------------------------
 * Includes
 * ------------------------------
 */
  require_once '../config.php';
  require_once './library/OneArchy/Session.php';
  require_once './library/OneArchy/Database.php';

  $db = null;
  $query = '';
  $results = null;
  $id = -1;
  $quantity = -1;
  $pressXToContinue = true;
  $options = array();
  $arrArticles = array();
  $arrUpdated = array();

  try {
    OneArchySession::start();

    if (OneArchySession::exists('shopcartItems')) {
      $arrArticles = OneArchySession::get('shopcartItems');
    }
    else {
      OneArchySession::set('shopcartItems', []);
    }

    if (isset($_GET['id']) && isset($_GET['quantity']) && $_GET['id'] !== '' && $_GET['quantity'] !== '') {
      $id = intval($_GET['id']);
      $quantity = intval($_GET['quantity']);

      if ($quantity > 1000) {
        $quantity = 1000;
      }

      if (isset($_GET['options']) && gettype($_GET['options']) === 'array') {
        $tempOptions = $_GET['options'];

        if (count($tempOptions) > 0) {
          foreach ($tempOptions as $option) {
            $tempSplit = explode('x', $option);

            if (count($tempSplit) > 1 && intval($tempSplit[1]) > 0) {
              // Add as quantifier - identifier pair
              $options[] = ['quantifier' => intval($tempSplit[0]), 'id' => intval($tempSplit[1])];
            }
          }
        }
      }

      if ($id > 0 && $quantity > 0) {
        $db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
  			$db->connect();

        foreach ($options as $option) {
          $query = 'SELECT COUNT(*) AS count FROM Products_has_Options WHERE productId = ? AND type_quantifier = ? AND optionId = ?';
          $results = $db->queryOne($query, [$id, $option['quantifier'], $option['id']]);

          if (intval($results['count']) < 1) {
            $pressXToContinue = false;
          }
        }

        if ($pressXToContinue) {
          echo '<pre>';
          var_dump($_SESSION);

          // Add to session
          if (OneArchySession::exists('shopcartItems') && count(OneArchySession::get('shopcartItems')) > 0) {
            // Extract, add and reinsert
            $arrArticles = OneArchySession::get('shopcartItems');
            $isPresent = false;

            foreach ($arrArticles as $article) {
              if ($article['id'] === $id && $article['options'] === $options) {
                $arrUpdated[] = ['id' => $article['id'], 'quantity' => intval($article['quantity']) + $quantity, 'options' => $options];
                $isPresent = true;
              }
              else {
                $arrUpdated[] = $article;
              }
            }

            if (!$isPresent) {
              $arrUpdated[] = ['id' => $id, 'quantity' => $quantity, 'options' => $options];
            }

            OneArchySession::set('shopcartItems', $arrUpdated);
          }
          else {
            OneArchySession::set('shopcartItems', [['id' => $id, 'quantity' => $quantity, 'options' => $options]]);
          }

          var_dump(null);
          var_dump($_SESSION);
          echo '</pre>';
        }
      }
    }
  }
  catch (Exception $e) {
    if ($db !== null) {
      $db->logError('Er is een fout opgetreden bij het toevoegen van een artikel aan de winkelmand - sabotage?', ['exception' => $e->getMessage(), 'query' => $query, 'results' => $results, 'id' => $id, 'quantity' => $quantity, 'options' => $options, 'arrArticles' => $arrArticles, 'arrUpdated' => $arrUpdated, 'site' => 'addToCart']);
    }
  }

  header('Location: index.php');
?>
