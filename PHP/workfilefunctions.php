<?php
private constant $APIKEY = "bfeb66d29afeca7b6b4229bc7bd42938";

public function nabMovieInfo($tmdbId) {
	// request info hier
	$arrPopular = requestPopular();		// Check surface to see which to use (detailed information enough to fill?)
	$objPopTarget;
	
	foreach ($objPopular in $arrPopular) {
		if ($objPopular['Id'] == $tmdbId) {
			$objPopTarget = $objPopular;
		}
	
		$objMovie = array(
			'Title' => $objPopTarget['title'],
			'Description' => $objPopTarget['overview'],
			'Release' => $objPopTarget['release'],
			'Thumb' => $objPopTarget['thumb_path'],
			'Homepage' => $objPopTarget['url'],
			'IMDB' => $objPopTarget['imdb'],
			'Runtime' => $objPopTarget['runtime'],
			'Language' => $objPopTarget['language'],
			'Status' => $objPopTarget['status'],
			'TMDBId' => $objPopTarget['id']
		);
		
		if(!movieExists($tmdbId) {
			// Do some nabby stuff, request all other info, put it into objects
			$objDetail = requestDetail($tmdbId);
			$objCast = requestCast($tmdbId);
			// HOW ARE WE SUPPOSED TO FILL THE CAST TABLE? WHO IS IMPORTANT ENOUGH - credit search hopelessly bugged - top 30? all + enum(major,minor,cameo)? -> manual check when films < 10 (+2 tables ActorsInTransit & MinorActors +1 page admin)
			
			// Do some putty-on-y stuff
		}
	}
}

public function requestPopular() {
	$curl = curl_init();

	curl_setopt_array($curl, array(
		CURLOPT_URL => "https://api.themoviedb.org/3/discover/movie?sort_by=popularity.desc&api_key=" . $APIKEY,
		CURLOPT_RETURNTRANSFER => true,
		CURLOPT_ENCODING => "",
		CURLOPT_MAXREDIRS => 10,
		CURLOPT_TIMEOUT => 30,
		CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
		CURLOPT_CUSTOMREQUEST => "GET",
		CURLOPT_POSTFIELDS => "{}",
	));

	$response = curl_exec($curl);
	$err = curl_error($curl);
	curl_close($curl);

	if ($err) {
		echo "cURL Error #:" . $err;
	} else {
		$decodedResponse = json_decode($response, true);
		$results = $decodedResponse['results'];  //multiple (discover)
	}
	
	return $tmdbPopular;
}

public function requestByQuery($query) {
	$curl = curl_init();

	curl_setopt_array($curl, array(
		CURLOPT_URL => "https://api.themoviedb.org/3/search/movie?include_adult=false&query=a&language=en-US&api_key=" . $APIKEY,
		CURLOPT_RETURNTRANSFER => true,
		CURLOPT_ENCODING => "",
		CURLOPT_MAXREDIRS => 10,
		CURLOPT_TIMEOUT => 30,
		CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
		CURLOPT_CUSTOMREQUEST => "GET",
		CURLOPT_POSTFIELDS => "{}",
	));

	$response = curl_exec($curl);
	$err = curl_error($curl);
	curl_close($curl);

	if ($err) {
		echo "cURL Error #:" . $err;
	} else {
		$decodedResponse = json_decode($response, true);
		$results = $decodedResponse['results'];

		foreach ($results as $result)
		{
			$dvd = array ('Title' => $result['title'], 'Description' => $result['overview'], 'Release' => $result['release_date'], 'Thumb' => $result['poster_path']);
		}
	}
}

public function requestCast($movieId) {
	$movieId = (int) $movieId;
	$creditsCurl = curl_init();

	curl_setopt_array($creditsCurl, array(
		CURLOPT_URL => "https://api.themoviedb.org/3/movie/" . $movieId . "/credits?api_key=" . $APIKEY,
		CURLOPT_RETURNTRANSFER => true,
		CURLOPT_ENCODING => "",
		CURLOPT_MAXREDIRS => 10,
		CURLOPT_TIMEOUT => 30,
		CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
		CURLOPT_CUSTOMREQUEST => "GET",
		CURLOPT_POSTFIELDS => "{}",
	));

	$creditsResponse = curl_exec($creditsCurl);
	$creditsErr = curl_error($creditsCurl);
	$directorId = 0;
	curl_close($creditsCurl);

	if ($creditsErr) {
		echo "cURL Error #:" . $creditsErr;
	} else {
		$creditsDecoded = json_decode($creditsResponse, true);
		$arrCast = (array) $creditsDecoded['cast'];
		$arrCrew = (array) $creditsDecoded['crew'];

		foreach ($arrCrew as $crew)
		{
			if ($crew['job'] == 'Director')
			{
				$directorId = (int) $crew['id'];
				$directorCurl = curl_init();

				curl_setopt_array($directorCurl, array(
					CURLOPT_URL => "https://api.themoviedb.org/3/person/" . $directorId . "?api_key=" . $APIKEY,
					CURLOPT_RETURNTRANSFER => true,
					CURLOPT_ENCODING => "",
					CURLOPT_MAXREDIRS => 10,
					CURLOPT_TIMEOUT => 30,
					CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
					CURLOPT_CUSTOMREQUEST => "GET",
					CURLOPT_POSTFIELDS => "{}",
				));

				$directorResponse = curl_exec($directorCurl);
				$directorErr = curl_error($directorCurl);
				curl_close($directorCurl);

				if ($directorErr) {
					echo "cURL Error #:" . $directorErr;
				} else {
					$directorDecoded = json_decode($directorResponse, true);
					//$results = $directorDecoded['results'];
				}
			}
		}
	}
}

public function requestDetail($tmdbId) {
	$tmdbId = (int) $tmdbId;
	
	// CURL stuff
	$movieCurl = curl_init();

	curl_setopt_array($movieCurl, array(
		CURLOPT_URL => "https://api.themoviedb.org/3/movie/" . $tmdbId . "?api_key=" . $APIKEY,
		CURLOPT_RETURNTRANSFER => true,
		CURLOPT_ENCODING => "",
		CURLOPT_MAXREDIRS => 10,
		CURLOPT_TIMEOUT => 30,
		CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
		CURLOPT_CUSTOMREQUEST => "GET",
		CURLOPT_POSTFIELDS => "{}",
	));

	$movieResponse = curl_exec($movieCurl);
	$movieErr = curl_error($movieCurl);
	curl_close($movieCurl);

	if ($movieErr) {
		echo "cURL Error #:" . $movieErr;
		//header('location: error.php?error=' . $movieErr);
	} else {
		$movieDecoded = json_decode($movieResponse, true);
	}
	
	return $tmdbMovie;
}

public function requestPerson($tmdbId) {
	$tmdbId = (int) $tmdbId;
	
	// CURL stuff
	$movieCurl = curl_init();

	curl_setopt_array($movieCurl, array(
		CURLOPT_URL => "https://api.themoviedb.org/3/person/" . $tmdbId . "?api_key=" . $APIKEY,
		CURLOPT_RETURNTRANSFER => true,
		CURLOPT_ENCODING => "",
		CURLOPT_MAXREDIRS => 10,
		CURLOPT_TIMEOUT => 30,
		CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
		CURLOPT_CUSTOMREQUEST => "GET",
		CURLOPT_POSTFIELDS => "{}",
	));

	$movieResponse = curl_exec($movieCurl);
	$movieErr = curl_error($movieCurl);
	curl_close($movieCurl);

	if ($movieErr) {
		echo "cURL Error #:" . $movieErr;
		//header('location: error.php?error=' . $movieErr);
	} else {
		$movieDecoded = json_decode($movieResponse, true);
	}
	
	return $tmdbPerson;
}

// Returns user id if set in session, and is called sessionCheck in order to not give away its functionality for privacy purposes
public function sessionCheck() {
	if (isset($_SESSION['user']) && is_numeric($_SESSION['user'])) {
		return (int) $_SESSION['user'];
	}
	else {
		return 0;
	}
}

public function insertMovie($objMovie) {
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	$strKeys = '';
	$strValues = '';
	
	// Check if everything is there already
	if (!movieExists((string) $objMovie["TMDBId"])) {
		$blnFirst = true;
		
		// Insert because it doesn't exist yet
		foreach ($objMovie as $key=>$value) {
			if ($blnFirst) {
				$strKeys = $key;
				$strValues = $value;
				$blnFirst = false;
			}
			else {
				$strKeys = $strKeys . ',' . $key;
				$strValues = $strValues . ',' . $value;
			}
		}
		
		$query = 'INSERT INTO Movies(' . $strKeys . ') VALUES(' . $strValues . ')';
		$result = $plonkObj->execute($query);
	}
}

public function insertActor($objActor) {
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	$strKeys = '';
	$strValues = '';
	
	// Check if everything is there already
	$query = 'SELECT * FROM Actors WHERE Name LIKE "' . htmlentities(stripslashes((string) $objActor["Name"]) . '" AND DateOfBirth LIKE ' . htmlentities(stripslashes((string) $objActor["DateOfBirth"])) . '"';
	$result = $plonkObj->retrieve($query);
	
	switch((int) $result[0]) {
		case 0:
			$blnFirst = true;
			
			// Insert because it doesn't exist yet
			foreach ($objActor as $key=>$value) {
				if ($blnFirst) {
					$strKeys = $key;
					$strValues = $value;
					$blnFirst = false;
				}
				else {
					$strKeys = $strKeys . ',' . $key;
					$strValues = $strValues . ',' . $value;
				}
			}
			
			$query = 'INSERT INTO Actors(' . $strKeys . ') VALUES(' . $strValues . ')';
			$result = $plonkObj->execute($query);
			break;
		default:
			// Don't insert
			break;
	}
}

public function insertDirector($objDirector) {
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	$strKeys = '';
	$strValues = '';
	
	// Check if everything is there already
	$query = 'SELECT * FROM Directors WHERE Name LIKE "' . htmlentities(stripslashes((string) $objDirector["Name"]) . '" AND DateOfBirth LIKE "' . htmlentities(stripslashes($objDirector["DateOfBirth"])) . '"';
	$result = $plonkObj->retrieve($query);
	
	// Insert here, at least if there is no such director
	switch((int) $result[0]) {
		case 0:
			$blnFirst = true;
			
			// Insert because it doesn't exist yet
			foreach ($objDirector as $key=>$value) {
				if ($blnFirst) {
					$strKeys = $key;
					$strValues = $value;
					$blnFirst = false;
				}
				else {
					$strKeys = $strKeys . ',' . $key;
					$strValues = $strValues . ',' . $value;
				}
			}
			
			$query = 'INSERT INTO Directors(' . $strKeys . ') VALUES(' . $strValues . ')';
			$result = $plonkObj->execute($query);
			break;
		default:
			// Don't insert
			break;
	}
}

public function insertGenre($name, $tmdbId) {
	$name = (string) $name;
	$tmdbId = (string) $tmdbId;
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	
	// Check if everything is there already
	$query = 'SELECT * FROM Genres WHERE Title LIKE "' . htmlentities(stripslashes($name) . '"';
	$result = $plonkObj->retrieve($query);
	
	// Insert here, at least if there is no such genre
	switch((int) $result[0]) {
		case 0:
			// Insert
			$query = 'INSERT INTO Genres(Name, TMDBId) VALUES(' . $name . ', ' . $tmdbId . ')';
			$result = $plonkObj->execute($query);
			
			break;
		default:
			// Don't insert
			break;
	}
}

public function insertCompany($name, $dateOfBirth) {
	$name = (string) $name;
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	
	// Check if everything is there already
	$query = 'SELECT * FROM Companies WHERE Name LIKE "' . htmlentities(stripslashes($name) . '"';
	$result = $plonkObj->retrieve($query);
	
	// Insert here, at least if there is no such company
	switch((int) $result[0]) {
		case 0:
			// Insert
			$query = 'INSERT INTO Companies(Name, DateOfBirth) VALUES(' . $name . ', ' . $dateOfBirth . ')';
			$result = $plonkObj->execute($query);
			
			break;
		default:
			// Don't insert
			break;
	}
}

// 0 = no f*cks given, 1 = DVD, 2 = BluRay
public function insertWishlisted($dvdId, $type = 0) {
	$dvdId = (int) $dvdId;
	$userId = (int) sessionCheck();
	$type = (int) $type;
	$query = '';
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	
	// Check if everything is there
	if ($userId == 0) {
		throw new Exception('You can&lsquo;t work this kind of magic without a user id, bub.')
		return false;
	}
	
	// Trying to double-wishlist? Now that's ballsy!
	if (wishlistedExists($dvdId, $userId)) {
		return false;
	}
	
	if (!movieExists($dvdId)) {
		return false;
	}
	
	// Insert here
	switch($type)
	{
		case 2:
			$query = 'INSERT INTO Wishlisted(UserId,DVDId,Type) VALUES(' . $userId . ',' . $dvdId . ',BluRay)';
			break;
		default:
			// type = 0 means none was given; type = 1 means the user selected a DVD, which is set to the default value in our DB and 2 = BluRay, which is the only explicit value in this equation
			$query = 'INSERT INTO Wishlisted(UserId,DVDId,Type) VALUES(' . $userId . ',' . $dvdId . ',DVD)';
			break;
	}
	
	return true;
}

// 0 = no f*cks given, 1 = DVD, 2 = BluRay
public function insertOwned($dvdId, $type = 0) {
	$dvdId = (int) $dvdId;
	$userId = (int) sessionCheck();
	$type = (int) $type;
	$query = '';
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	
	if ($userId > 0) {
		if (ownedExists($dvdId, $userId)) {
			typeUpdate($dvdId, $userId, $type);
			return 0;
		}
		else {
			switch($type)
			{
				case 2:
					$query = 'INSERT INTO Owned(UserId,DVDId,Type) VALUES(' . $userId . ',' . $dvdId . ',BluRay)';
					break;
				default:
					// type = 0 means none was given; type = 1 means the user selected a DVD, which is set to the default value in our DB and 2 = BluRay, which is the only explicit value in this equation
					$query = 'INSERT INTO Owned(UserId,DVDId,Type) VALUES(' . $userId . ',' . $dvdId . ',DVD)';
					break;
			}
			
			$result = $plonkObj->execute($query);
			
			return 1;
		}
	}
}

public function moveWishlisted($wishId, $type) {
	$wishId = (int) $wishId;
	$type = (int) $type;
	
	// select id from wishlisted and insert into Owned
	insertOwned($dvdId, $type);
	deleteWishlisted($wishId);
}

public function typeUpdate($dvdId, $userId, $type) {
	$dvdId = (int) $dvdId;
	$userId = (int) $userId;
	$type = (int) $type;
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	
	if (wishlistedExists($dvdId, $userId)) {
		$query = 'UPDATE Owned SET Type = ' . htmlentities(stripslashes($type)) . ' WHERE DVDId = ' . htmlentities(stripslashes($dvdid)) . ' AND UserId = ' . htmlentities(stripslashes($userId));
		$result = $plonkObj->execute($query);
	}
}

public function moveToExtra($actorId) {
	$actorId = (int) $actorId;
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	
	// No need to check, just update
	$query = 'UDPATE Actors SET InTransit = 0, Extra = 1 WHERE Id = ' . htmlentities(stripslashes($actorId));
	$result = $plonkObj->execute($query);
}

public function moveToMain($actorId) {
	$actorId = (int) $actorId;
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	
	// No need to check, just update
	$query = 'UDPATE Actors SET InTransit = 0, Extra = 0 WHERE Id = ' . htmlentities(stripslashes($actorId));
	$result = $plonkObj->execute($query);
}

public function deleteOwned($dvdId, $userId) {
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	
	// Checks whether or not the movie exists first - can't be owned unless it exists
	If (!movieExists($dvdId)) {
		return false;
	}
	
	// Checks (user check useless, shouldn't have to delete stuff when a user doesn't exist)
	if (!ownedExists($dvdId, $userId)) {
		return false;
	}
	
	// Delete here, you'll not get here in case anything's wrong
	$query = 'DELETE FROM Owned WHERE DVDId = ' . $dvdId . ' AND UserId = ' . $userId;
	$result = $plonkObj->execute($query);
	
	return true;
}

public function deleteWishlisted($dvdId, $userId) {
	$dvdId = (int) $dvdId;
	$userId = (int) $userId;
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	
	// Checks whether or not the movie exists first - can't be wishlisted unless it exists
	If (!movieExists($dvdId)) {
		return false;
	}
	
	// Checks (user check useless, shouldn't have to delete stuff when a user doesn't exist)
	if (!wishlistedExists($dvdId, $userId)) {
		return false;
	}
	
	// Delete here, you'll not get here in case anything's wrong
	$query = 'DELETE FROM Wishlisted WHERE DVDId = ' . $dvdId . ' AND UserId = ' . $userId;
	$result = $plonkObj->execute($query);
	
	return true;
}

public function movieExists($tmdbId) {
	$tmdbId = (int) $tmdbId;
	$query = 'SELECT COUNT(*) FROM DVDs WHERE tmdbId = ' . $tmdbId;
	$result = $plonkObj->retrieveOne($query);

	switch((int) $result[0])
	{
		case 1:
			return true;
			break;
		case 0:
			return false;
			break;
		default:
			throw new Exception('There shouldn&lsquo;t be this many movies with a single id. Count = ' . $result['Total']);
			break;
	}
	
	return false;
}

public function ownedExists($dvdId, $userId) {
	$dvdId = (int) $dvdId;
	$userId = (int) $userId;
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	
	$query = 'SELECT COUNT(*) FROM Owned WHERE DVDId = ' . $dvdId . ' AND UserId = ' . $userId . ' AND Type = "DVD"';
	$dvdResult = $plonkObj->retrieve($query);
	$query = 'SELECT COUNT(*) FROM Owned WHERE DVDId = ' . $dvdId . ' AND UserId = ' . $userId . ' AND Type = "BluRay"';
	$bluResult = $plonkObj->retrieve($query);
	
	switch((int) $bluResult[0])
	{
		case 1:
			return 2;
			break;
		case 0:
			if ((int) $dvdResult[0] = 1) {
				return 1;
			}
			else {
				if ((int) $dvdResult[0] > 1) {
					throw new Exception('There shouldn&lsquo;t be this many owned movies with a single pair of ids. Count = ' . $dvdResult['Total']);
				}
				
				return 0;
			}
		default:
			throw new Exception('There shouldn&lsquo;t be this many owned movies with a single pair of ids. Count = ' . $bluResult['Total']);
			break;
	}
}

public function wishlistedExists($dvdId, $userId) {
	$dvdId = (int) $dvdId;
	$userId = (int) $userId;
	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
	$plonkObj->connect();
	
	$query = 'SELECT COUNT(*) FROM Wishlisted WHERE DVDId = ' . $dvdId . ' AND UserId = ' . $userId . ' AND Type = "DVD"';
	$dvdResult = $plonkObj->retrieve($query);
	$query = 'SELECT COUNT(*) FROM Wishlisted WHERE DVDId = ' . $dvdId . ' AND UserId = ' . $userId . ' AND Type = "BluRay"';
	$bluResult = $plonkObj->retrieve($query);
	
	switch((int) $bluResult[0])
	{
		case 1:
			return 2;
			break;
		case 0:
			if ((int) $dvdResult[0] = 1) {
				return 1;
			}
			else {
				if ((int) $dvdResult[0] > 1) {
					throw new Exception('There shouldn&lsquo;t be this many wishlisted movies with a single pair of ids. Count = ' . $dvdResult['Total']);
				}
				
				return 0;
			}
		default:
			throw new Exception('There shouldn&lsquo;t be this many wishlisted movies with a single pair of ids. Count = ' . $bluResult['Total']);
			break;
	}
}