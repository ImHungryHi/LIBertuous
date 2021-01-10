<?php

/**
 * Database klasse om de homepage te voorzien van de nodige data
 * -------------------------------------------------------------
 */

class HomeDB
{
  /**
   * Queries the database and returns a list of all products
   * @return array
   */
  public static function getAllProducts()
  {
		$products = array();

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			$query = 'SELECT * FROM Products';
			$products = $db->queryAll($query, []);
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van producten.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'products' => $products, 'site' => 'home']);
      }

			throw new Exception($errorMessage);
		}

	  return $products;
  }

  /**
   * Queries the database and returns the products in a category
   * @param string $category
   * @return array
   */
  public static function getProductsByCategory($category)
  {
    $results = array();

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			$query = 'SELECT * FROM Products WHERE type = ?';
			$results = $db->queryAll($query, [$category]);
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van gecategoriseerde producten.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'category' => $category, 'results' => $results, 'site' => 'home']);
      }

			throw new Exception($errorMessage);
		}

	  return $results;
  }

  /**
   * Queries the database and returns the options for a product
   * @param int $productId
   * @return array
   */
  public static function getOptionsForProduct($productId)
  {
		$productId = intval($productId);
    $results = array();
    $parsedResults = array();

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

      $query = 'SELECT Options.id AS id, Options.name AS name, Options.price AS price, Products_has_Options.type AS type, Products_has_Options.type_quantifier AS type_quantifier, Products_has_Options.type_identifier AS type_identifier, Products_has_Options.isMandatory AS isMandatory
        FROM Products_has_Options INNER JOIN Options ON Products_has_Options.optionId = Options.id
          WHERE Products_has_Options.productId = ?';
      $results = $db->queryAll($query, [$productId]);

      foreach ($results as $result) {
        $parsedResults[$result['type_quantifier']]['type'] = $result['type'];
        $parsedResults[$result['type_quantifier']]['type_quantifier'] = intval($result['type_quantifier']);
        $parsedResults[$result['type_quantifier']]['type_identifier'] = $result['type_identifier'];
        $parsedResults[$result['type_quantifier']]['isMandatory'] = $result['isMandatory'];
        $parsedResults[$result['type_quantifier']]['rows'][] = ['id' => intval($result['id']), 'name' => $result['name'], 'price' => floatval($result['price'])];
      }
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van opties voor een product.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'productId' => $productId, 'results' => $results, 'parsedResults' => $parsedResults, 'site' => 'home']);
      }

			throw new Exception($errorMessage);
		}

	  return $parsedResults;
  }

  /**
   * Queries the database and returns the price of a product
   * @param int $id
   * @return float
   */
  public static function getProductPrice($id)
  {
		$id = intval($id);

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			$query = 'SELECT price FROM Products WHERE id = ?';
			$results = $db->queryOne($query, [$id]);
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van een productprijs.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'id' => $id, 'query' => $query, 'results' => $results, 'site' => 'home']);
      }

			throw new Exception($errorMessage);
		}

	  return floatval($results['price']);
  }

  /**
   * Queries the database and returns the price of an option
   * @param int $optionId
   * @return float
   */
  public static function getPriceForOption($optionId)
  {
		$optionId = intval($optionId);

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			$query = 'SELECT price FROM Options WHERE id = ?';
			$results = $db->queryOne($query, [$optionId]);
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van een optieprijs.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'optionId' => $optionId, 'results' => $results, 'site' => 'home']);
      }

			throw new Exception($errorMessage);
		}

	  return floatval($results['price']);
  }

  /**
   * Queries the database and returns all product categories
   * @return array
   */
  public static function getCategories()
  {
		$results = array();

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			$query = 'SELECT DISTINCT type FROM Products';
			$results = $db->queryAll($query, []);
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van alle categorieën.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'results' => $results, 'site' => 'home']);
      }

			throw new Exception($errorMessage);
		}

	  return $results;
  }
}
?>
