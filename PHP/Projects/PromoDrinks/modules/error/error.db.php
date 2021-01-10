<?php
/**
 * And you wondered where the model went? Well, look no further. Actually do, the stuff below is pretty interesting
 */
class ErrorDB {
  // Voorlopig geen interesse in foutlogging richting database, kan later eventueel wel geïmplementeerd worden.

	/**
	 * Gets option price from the database
	 * @param int $id
	 * @return int
	 */
	public static function getOptionPrice($id)
	{
	  // rework params
	  $id = (int) $id;

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			$query = 'SELECT price FROM Options WHERE id = ?';
			$price = $db->queryOne($query, [$id]);
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van een optieprijs.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'results' => $price, 'id' => $id, 'site' => 'error']);
      }

			throw new Exception($errorMessage);
		}

	  return $price['price'];
	}

	/**
	 * Gets sauce price from the database
	 * @param int $id
	 * @return int
	 */
	public static function getProductPrice($id)
	{
	  // rework params
	  $id = (int) $id;

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			$query = 'SELECT price FROM Products WHERE id = ?';
			$price = $db->queryOne($query, [$id]);
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van een productprijs.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'results' => $price, 'id' => $id, 'site' => 'error']);
      }

			throw new Exception($errorMessage);
		}

	  return $price['price'];
	}
}
?>
