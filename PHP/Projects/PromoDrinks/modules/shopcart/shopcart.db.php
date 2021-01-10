<?php
/**
 * And you wondered where the model went? Well, look no further. Actually do, the stuff below is pretty interesting
 */
class ShopcartDB {
	/**
	 * Gets article information based on article IDs
	 * @param array $ids
	 * @return array
	 */
	public static function getProductInfoByIds($ids) {
		$ids = (array) $ids;
		$parsedIds = array();
		$results = array();

		// Reparse individually
		foreach ($ids as $id) {
			$parsedIds[] = intval($id);
		}

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			$parameterLine = '(' . implode(',', array_fill(0, count($ids), '?')) . ')';
			$query = 'SELECT * FROM Products WHERE id in ' . $parameterLine;
			$results = $db->queryAll($query, $parsedIds);
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van artikelen.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'results' => $results, 'ids' => $ids, 'parsedIds' => $parsedIds, 'site' => 'shopcart']);
      }

			throw new Exception($errorMessage);
		}

	  return $results;
	}

	/**
	 * Gets article options based on article IDs
	 * @param array $ids
	 * @return array
	 */
	public static function getProductOptions($ids) {
		$ids = (array) $ids;
		$parsedIds = array();
		$results = array();

		// Reparse individually
		foreach ($ids as $id) {
			$parsedIds[] = intval($id);
		}

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			$parameterLine = '(' . implode(',', array_fill(0, count($ids), '?')) . ')';
			$query = 'SELECT Products_has_Options.productId AS productId, Products_has_Options.isMandatory AS isMandatory, Products_has_Options.type AS type,
				Products_has_Options.type_quantifier AS type_quantifier, Products_has_Options.type_identifier AS type_identifier,
				Options.id AS optionId, Options.name AS optionName, Options.price AS optionPrice
				FROM Products_has_Options INNER JOIN Options ON Products_has_Options.optionId = Options.id
					WHERE productId in ' . $parameterLine;
			$results = $db->queryAll($query, $parsedIds);
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van opties.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'results' => $results, 'ids' => $ids, 'parsedIds' => $parsedIds, 'site' => 'shopcart']);
      }

			throw new Exception($errorMessage);
		}

	  return $results;
	}
}
?>
