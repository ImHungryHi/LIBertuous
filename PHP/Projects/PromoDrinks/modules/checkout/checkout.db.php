<?php
/**
 * And you wondered where the model went? Well, look no further. Actually do, the stuff below is pretty interesting
 */
class CheckoutDB {
	/**
	 * Queries the database and returns the price of an option
	 * @param int $optionId
	 * @return float
	 */
	public static function getPriceForOption($optionId) {
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
				$db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'optionId' => $optionId, 'results' => $results, 'site' => 'checkout']);
			}

			throw new Exception($errorMessage);
		}

		return floatval($results['price']);
	}

	/**
	 * Gets sauce price from the database
	 * @param int $id
	 * @return int
	 */
	public static function getProductPrice($id) {
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
				$db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'results' => $price, 'productId' => $id, 'site' => 'checkout']);
			}

			throw new Exception($errorMessage);
		}

		return $price['price'];
	}

	/**
	 * Gets article information based on ID
	 * @param int $id
	 * @return array
	 */
	public static function getProductInfoById($id) {
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
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'results' => $results, 'id' => $id, 'site' => 'checkout']);
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
	public static function getOptionInfoByIds($ids) {
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
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'results' => $results, 'ids' => $ids, 'parsedIds' => $parsedIds, 'site' => 'checkout']);
      }

			throw new Exception($errorMessage);
		}

	  return $results;
	}

	/**
	 * Inserts the necessary information into the database to make an order. Returns the order ID on success
	 * @param array $personalData
	 * @param array $articleInfo
	 * @param string $comment
	 * @return int
	 */
	public static function insertOrder($personalData, $articleInfo, $comment = '')
	{
	  // Rework params
	  $personalData = (array) $personalData;
	  $articleInfo = (array) $articleInfo;
	  $comment = (string) $comment;

		// Initialize some objects for use
		$orderId = -1;
		$query = '';
		$clientId = -1;
		$orderProductId = -1;
		$arrOptions = array();
		$tempOptionIds = array();
		$columnsLine = '(lastName, firstName, email, phoneNumber, addressLine1, postal1, city1)';
		$valuesLine = '(?, ?, ?, ?, ?, ?, ?)';
		$clientInfo = [
			$personalData['lastName'], // lastName
			$personalData['firstName'], // firstName
			$personalData['mail'], // email
			$personalData['phone'], // phoneNumber
			$personalData['address'], // addressLine1
			$personalData['postal'], // postal1
			$personalData['city'] // city1
		];

		if (isset($personalData['multiAddress']) && $personalData['multiAddress']) {
			$columnsLine = '(lastName, firstName, email, phoneNumber, addressLine1, postal1, city1, addressLine2, postal2, city2)';
			$valuesLine = '(?, ?, ?, ?, ?, ?, ?, ?, ?, ?)';
			$clientInfo = [
				$personalData['lastName'], // lastName
				$personalData['firstName'], // firstName
				$personalData['mail'], // email
				$personalData['phone'], // phoneNumber
				$personalData['address'], // addressLine1
				$personalData['postal'], // postal1
				$personalData['city'], // city1
				$personalData['addressExtra'], // addressLine1
				$personalData['postalExtra'], // postal1
				$personalData['cityExtra'] // city1
			];
		}

		try {
			$db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
			$db->connect();

			// Checking if the client already exists
			$query = 'SELECT id FROM Clients WHERE firstName = ? AND lastName = ? AND addressLine1 = ?';
			$result = $db->queryOne($query, [$personalData['firstName'], $personalData['lastName'], $personalData['address']]);

			// Adding a client if it doesn't exist
			if ($result === false) {
				$query = 'INSERT INTO Clients ' . $columnsLine . ' VALUES ' . $valuesLine;
				$result = $db->queryOne($query, $clientInfo);
				$clientId = $db->getLastId();
			}
			elseif (gettype($result['id']) === 'integer') {
				$clientId = $result['id'];

				if ($result['addressLine2'] !== $personalData['addressExtra']) {
					$query = 'UPDATE Clients SET addressLine2 = ?, postal2 = ?, city2 = ? WHERE id = ?';
					$result = $db->queryOne($query, [$personalData['addressExtra'], $personalData['postalExtra'], $personalData['cityExtra'], $clientId]);
				}
			}

			// Adding an order
			$columnsLine = '(clientId, comment, orderedOn)';
			$valuesLine = '(?, ?, utc_timestamp())';

			$query = 'INSERT INTO Orders ' . $columnsLine . ' VALUES ' . $valuesLine;
			$result = $db->queryOne($query, [$clientId, $comment]);
			$orderId = $db->getLastId();

			foreach ($articleInfo as $article) {
				$arrOptions = array();

				if (isset($article['options']) && count($article['options']) > 0) {
					$tempOptionIds = array_column($article['options'], 'id');
					$query = 'SELECT id FROM Products_has_Options WHERE productId = ' . $article['id'] . ' AND optionId IN (' . implode(',', array_fill(0, count($tempOptionIds), '?')) . ')';
					$arrOptions = $db->queryAll($query, $tempOptionIds);
				}

				$query = 'INSERT INTO Orders_has_Products (orderId, productId, quantity) VALUES (?, ?, ?)';
				$result = $db->queryOne($query, [$orderId, $article['id'], $article['quantity']]);
				$orderProductId = $db->getLastId();

				if ($orderProductId !== null && $orderProductId !== false && $arrOptions !== [] && $arrOptions !== false) {
					$query = 'INSERT INTO Orders_has_Products_has_Options (orderProductId, productOptionId) VALUES (?, ?)';

					foreach ($arrOptions as $productOptionId) {
						$result = $db->queryOne($query, [$orderProductId, $productOptionId['id']]);
					}
				}
			}
		}
		catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het toevoegen van een nieuwe klant en/of bestelling.';

      if ($db !== null) {
				$debugInfo = [
					'exception' => $e->getMessage(),
					'personalData' => $personalData,
					'comment' => $comment,
					'articleInfo' => $articleInfo,
					'clientInfo' => $clientInfo,
					'columnsLine' => $columnsLine,
					'valuesLine' => $valuesLine,
					'query' => $query,
					'result' => $result,
					'clientId' => $clientId,
					'orderId' => $orderId,
					'orderProductId' => $orderProductId,
					'arrOptions' => $arrOptions,
					'tempOptionIds' => $tempOptionIds,
					'site' => 'checkout'
				];

        $db->logError($errorMessage, $debugInfo);
      }

			throw new Exception($errorMessage);
		}

		return $orderId;
	}

	/**
	 * Gets client info based on order id
	 * @param int $orderId
	 * @return array
	 */
	public static function getClientInfoFromOrder($orderId) {
		$orderId = (int) $orderId;
		$clientInfo = [];

		if ($orderId < 1) {
			throw new Exception('Ongeldig bestelnummer opgegeven.');
		}

    try {
      $db = new OneArchyDB(PD_DB_HOST, PD_DB_NAME, PD_DB_USER, PD_DB_PASS);
      $db->connect();

      $query = 'SELECT Clients.firstName AS firstName, Clients.lastName AS lastName, Clients.email AS email, Clients.phoneNumber AS phoneNumber, Clients.addressLine1 AS addressLine1, Clients.postal1 AS postal1, Clients.city1 AS city1, Clients.addressLine2 AS addressLine2, Clients.postal2 AS postal2, Clients.city2 AS city2
        FROM Clients INNER JOIN Orders ON Clients.id = Orders.clientId WHERE Orders.id = ?';
      $clientInfo = $db->queryOne($query, [$orderId]);
    }
    catch (Exception $e) {
      $errorMessage = 'Er is een fout opgetreden bij het oproepen van klantinfo.';

      if ($db !== null) {
        $db->logError($errorMessage, ['exception' => $e->getMessage(), 'query' => $query, 'results' => $clientInfo, 'orderId' => $orderId, 'site' => 'checkout']);
      }

			throw new Exception($errorMessage);
    }

		return $clientInfo;
	}
}
?>
