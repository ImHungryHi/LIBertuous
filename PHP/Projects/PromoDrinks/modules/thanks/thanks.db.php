<?php
/**
 * And you wondered where the model went? Well, look no further. Actually do, the stuff below is pretty interesting
 */
class ThanksDB {
	/**
	 * Gets article information based on ID
	 * @param int $id
	 * @return array
	 */
	public static function getProductInfoById($id)
  {
    // rework params
	  $id = intval($id);
		$results = array();

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			$query = 'SELECT * FROM Products WHERE id = ?';
			$product = $db->queryOne($query, [$id]);
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van artikelinfo.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'results' => $results, 'id' => $id, 'site' => 'thanks']);
      }

			throw new Exception($errorMessage);
		}

    return $product;
	}

	/**
	 * Gets sauce information based on their IDs
	 * @param array $ids
	 * @return array
	 */
	public static function getOptionInfoByIds($ids)
	{
	  // rework params
	  $ids = (array) $ids;
		$parsedIds = array();
		$results = array();

		foreach ($ids as $id) {
			$parsedIds[] = intval($id);
		}

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			$parameterLine = '(' . implode(',', array_fill(0, count($parsedIds), '?')) . ')';
			$query = 'SELECT DISTINCT Options.id AS id, Options.name AS name, Options.price AS price, Products_has_Options.type_identifier AS type_identifier
				FROM Options INNER JOIN Products_has_Options ON Options.id = Products_has_Options.optionId
					WHERE Options.id IN ' . $parameterLine;
			$results = $db->queryAll($query, $ids);
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van optie infos.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'results' => $results, 'ids' => $ids, 'parsedIds' => $parsedIds, 'site' => 'thanks']);
      }

			throw new Exception($errorMessage);
		}

	  return $results;
	}
}
?>
