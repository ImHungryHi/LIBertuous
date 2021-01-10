<?php
/**
 * ------------------------------
 * Includes
 * ------------------------------
 */
  require_once '../config.php';
  require_once './library/OneArchy/Database.php';

  try {
    $optionIds = array();

    if (isset($_GET['ids']) && gettype($_GET['ids']) === 'array') {
      foreach ($_GET['ids'] as $id) {
        if (intval($id) > 0) {
          $optionIds[] = intval($id);
        }
      }

      if (count($optionIds) > 0) {
        $db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
  			$db->connect();
        $ins = '(' . implode(',', array_fill(0, count($optionIds), '?')) . ')';
        $query = 'SELECT id, price FROM Options WHERE id IN ' . $ins;
        $results = $db->queryAll($query, $optionIds);

        header('Content-type: application/json');
        echo json_encode($results, JSON_PRETTY_PRINT);
        //var_dump($results);
      }
    }
  }
  catch (Exception $e) {
    // Don't do anything
  }

  //header('Location: index.php');
?>
