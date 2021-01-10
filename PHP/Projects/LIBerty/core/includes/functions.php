<?php



    /**
     * Guaranteed slashes (if magic_quotes is off, it adds the slashes)
     *
     * @param string $string The string to add the slashes to
     * @return string
     */
    function addPostSlashes($string) {
	    if ((get_magic_quotes_gpc()==1) || (get_magic_quotes_runtime()==1))
		    return $string;
	    else return addslashes($string);
    }


    /**
     * Guaranteed no slashes (if magic_quotes is on, it strips the slashes)
     *
     * @param string $string The string to remove the slashes from
     * @return string
     */
    function stripPostSlashes($string) {
	    if ((get_magic_quotes_gpc()==1) || (get_magic_quotes_runtime()==1))
		    return stripslashes($string);
	    else return $string;
    }


    /**
     * Redirects to the error handling page
     * @param string $type
     * @param object $dbhandler
     * @return void
     */
    function showDbError($type, $dbhandler) {

	// debug activated
	if (DEBUG === true)
	{

	    switch($type)
	    {

		case 'connect':
		    echo mysqli_connect_error($dbhandler);
		break;

		case 'query':
		    echo mysqli_error($dbhandler);
		break;

		default:
		    echo 'There was an unknown error while communicating with the database';
		break;

	    }

	}

	// debug not activated
	else {

	    // The referrerd page will show a proper error based on the $_GET parameters
	    header('location: error.php?type=db&detail=' . $type);
	}

	exit();

    }

    function encrypt ($password) {
    	$cryptKey = 'd41d8cd98f00b204e9800998ecf8427e';
    	return base64_encode(mcrypt_encrypt(MCRYPT_RIJNDAEL_256, md5($cryptKey), $password, MCRYPT_MODE_CBC, md5(md5($cryptKey))));
    }

    function login ($username, $password) {
    	$username = (string) $username;
    	$password = (string) $password;
		$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
		$plonkObj->connect();

		$query = 'SELECT * FROM Users WHERE Name = "' . $username . '"';
		$result = $plonkObj->retrieveOne($query);

		if ($result == null)
		{
    		throw new Exception('User unknown, please use a correct username or register');
    	}
    	else
    	{
    		if ($result['Name'] === $username && $result['Password'] === encrypt($password))
    		{
    			return $result['Id'];
    		}
    	}

    	return 0;
    }

    function register ($username, $password) {
		$username = (string) $username;
		$password = (string) $password;
		$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
		$plonkObj->connect();

		$query = 'SELECT * FROM Users WHERE Name = "' . $username . '"';
		$result = $plonkObj->retrieveOne($query);

		if ($result == null)
		{
			$plonkObj->insert('Users', array('Name' => $username, 'Password' => encrypt($password)));
			$result = $plonkObj->retrieveOne($query);

			if ($result == null)
			{
				throw new Exception('User registration failed, our grease monkeys are on it');
			}
			else
			{
				return $result['Id'];
			}
		}
		else
		{
    		throw new Exception('Duplicate user, please login or use a different username');
    	}

    	return 0;
    }

    function userInDb ($id)
    {
    	$id = (int) $id;
    	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
    	$plonkObj->connect();

		$query = 'SELECT COUNT(*) AS Total FROM Users WHERE Id = ' . $id;
		$result = $plonkObj->retrieve($query);

		switch((int) $result[0])
		{
			case 0:
				return 0;
				break;
			case 1:
				return 1;
				break;
			default:
				throw new Exception('There shouldn&lsquo;t be this many users with a single id. Count = ' . $result['Total']);
		}

		return 0;
    }

    function getGenresByMovieId ($id)
    {
    	$id = (int) $id;
    	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
    	$plonkObj->connect();

    	$query = 'SELECT Genres.Id, Genres.Title FROM Genres INNER JOIN DVD_Has_Genres ON Genres.Id = DVD_Has_Genres.GenreId WHERE DVD_Has_Genres.DVDId = ' . $id;
		$result = $plonkObj->retrieve($query);

    	return $result;
    }

    function getDirectorsByMovieId ($id)
    {
    	$id = (int) $id;
    	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
    	$plonkObj->connect();

    	$query = 'SELECT Directors.Id, Directors.Name, Directors.DateOfBirth FROM Directors INNER JOIN DVD_Has_Directors ON Directors.Id = DVD_Has_Directors.DirectorId WHERE DVD_Has_Directors.DVDId = ' . $id;
    	$result = $plonkObj->retrieve($query);

    	return $result;
    }

    function getFilteredWishlist ($userid, $filter)
    {
    	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
    	$plonkObj->connect();
    	$userid = (int) $userid;
    	$filter = (string) @mysqli_escape_string($plonkObj, $filter);

		$query = 'SELECT DVDs.Id, DVDs.Title, DVDs.Description, DVDs.Release, DVDs.Thumb FROM DVDs INNER JOIN Wishlisted ON DVDs.Id = Wishlisted.DVDId WHERE Wishlisted.UserId = ' . $userid . ' AND DVDs.Title LIKE "%' . $filter . '%"';
		$result = $plonkObj->retrieve($query);

		return $result;
    }

    function getWishlisted ($userid)
    {
    	$userid = (int) $userid;
    	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
    	$plonkObj->connect();

    	$query = 'SELECT DVDs.Id, DVDs.Title, DVDs.Description, DVDs.Release, DVDs.Thumb FROM DVDs INNER JOIN Wishlisted ON DVDs.Id = Wishlisted.DVDId WHERE Wishlisted.UserId = ' . $userid;
    	$result = $plonkObj->retrieve($query);

    	return $result;
    }

    function getFilteredOwned ($userid, $filter)
    {
    	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
    	$plonkObj->connect();
    	$userid = (int) $userid;
    	$filter = (string) @mysqli_escape_string($plonkObj, $filter);

		$query = 'SELECT DVDs.Id, DVDs.Title, DVDs.Description, DVDs.Release, DVDs.Thumb FROM DVDs INNER JOIN Owned ON DVDs.Id = Owned.DVDId WHERE Owned.UserId = ' . $userid . ' AND DVDs.Title LIKE "%' . $filter . '%"';
		$result = $plonkObj->retrieve($query);

		return $result;
    }

    function getOwned ($userid)
    {
    	$userid = (int) $userid;
    	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
    	$plonkObj->connect();

    	$query = 'SELECT DVDs.Id, DVDs.Title, DVDs.Description, DVDs.Release, DVDs.Thumb FROM DVDs INNER JOIN Owned ON DVDs.Id = Owned.DVDId WHERE Owned.UserId = ' . $userid;
    	$result = $plonkObj->retrieve($query);

    	return $result;
    }

    function getMovies ()
    {
    	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
    	$plonkObj->connect();

    	$query = 'SELECT * FROM DVDs';
    	$result = $plonkObj->retrieve($query);

    	getTestMethod();
		//getTested();

    	return $result;
    }

    function getFilteredMovies ($filter)
   	{
   		$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
   		$plonkObj->connect();
    	$filter = (string) @mysqli_escape_string($plonkObj, $filter);

    	$query = 'SELECT * FROM DVDs WHERE Title LIKE "%' . $filter . '%"';
    	$result = $plonkObj->retrieve($query);

    	return $result;
    }

    function insertDVD($arrValues)
    {
    	$arrValues = (array) $arrValues;
    	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
   		$plonkObj->connect();

   		$query = 'SELECT COUNT(*) AS Total FROM DVDs WHERE Title LIKE "%' . $arrValues['Title'] . '%"';
   		$result = $plonkObj->retrieve($query);

		if ($result[0]['Total'] === '0')
   		{
   			// insert stuff
   			//$plonkObj->insert('DVDs', $arrValues);

   			if ($arrValues['Thumb'] != null && $arrValues['Thumb'] != '')
   			{
				//copy('https://image.tmdb.org/t/p/w300_and_h450_bestv2' . $arrValues['Thumb'], $_SERVER['DOCUMENT_ROOT'] . '/core/img/thumb' . $arrValues['Thumb']);
			}

   			return true;
   		}

   		return false;
    }

    function insertDirector($arrValues)
    {
    	$arrValues = (array) $arrValues;
    	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
   		$plonkObj->connect();

   		$query = 'SELECT COUNT(*) AS Total FROM Directors WHERE TMDBId = ' . $arrValues['TMDBId'];
   		$result = $plonkObj->retrieve($query);

   		if ($result[0]['Total'] === '0')
   		{
   			//$plonkObj->insert('Directors', $arrValues);

   			if ($arrValues['Profile'] != null && $arrValues['Profile'] != '')
   			{
   				//copy('https://image.tmdb.org/t/p/w600_and_h900_bestv2' . $arrValues['Profile'], $_SERVER['DOCUMENT_ROOT'] . '/core/img/profile' . $arrValues['Profile']);
   			}

   			return true;
   		}

   		return false;
    }

    function insertActor ($arrValues)
    {
    	$arrValues = (array) $arrValues;
    	$plonkObj = new PlonkDB(DB_HOST, DB_USER, DB_PASS, DB_NAME);
   		$plonkObj->connect();

   		$query = 'SELECT COUNT(*) AS Total FROM Actors WHERE TMDBId = ' . $arrValues['TMDBId'];
   		$result = $plonkObj->retrieve($query);

   		if ($result[0]['Total'] === '0')
   		{
   			//$plonkObj->insert('Actors', $arrValues);

   			if ($arrValues['Profile'] != null && $arrValues['Profile'] != '')
   			{
   				//copy('https://image.tmdb.org/t/p/w600_and_h900_bestv2' . $arrValues['Profile'], $_SERVER['DOCUMENT_ROOT'] . '/core/img/profile' . $arrValues['Profile']);
   			}

   			return true;
   		}

   		return false;
    }

    function getExternalMovies ()
    {
    	$curl = curl_init();

    	curl_setopt_array($curl, array(
    		CURLOPT_URL => "https://api.themoviedb.org/3/search/movie?include_adult=false&query=a&language=en-US&api_key=bfeb66d29afeca7b6b4229bc7bd42938",
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

		return NULL;
    }

    function nabMovie ($movieId)
    {
    	$movieId = (int) $movieId;
    	$movieCurl = curl_init();

    	curl_setopt_array($movieCurl, array(
    		CURLOPT_URL => "https://api.themoviedb.org/3/movie/" . $movieId . "?api_key=bfeb66d29afeca7b6b4229bc7bd42938",
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

    	$creditsCurl = curl_init();

    	curl_setopt_array($creditsCurl, array(
    		CURLOPT_URL => "https://api.themoviedb.org/3/movie/" . $movieId . "/credits?api_key=bfeb66d29afeca7b6b4229bc7bd42938",
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
						CURLOPT_URL => "https://api.themoviedb.org/3/person/" . $directorId . "?api_key=bfeb66d29afeca7b6b4229bc7bd42938",
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

    function getTested()
    {
    	$curli = curl_init();

    	curl_setopt_array($curli, array(
    		CURLOPT_URL => "https://api.themoviedb.org/3/movie/192136/credits?api_key=bfeb66d29afeca7b6b4229bc7bd42938",
    		CURLOPT_RETURNTRANSFER => true,
    		CURLOPT_ENCODING => "",
    		CURLOPT_MAXREDIRS => 10,
    		CURLOPT_TIMEOUT => 30,
    		CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
    		CURLOPT_CUSTOMREQUEST => "GET",
    		CURLOPT_POSTFIELDS => "{}",
    	));

		$detResponse = curl_exec($curli);
		$detErr = curl_error($curli);

		curl_close($curli);

		if ($detErr) {
			echo "cURL Error #:" . $detErr;
		} else {
			$decodedResponse = json_decode($detResponse, true);
			//$results = $decodedResponse['results'];  //multiple (discover)

			var_dump($decodedResponse);
		}

    	$curlo = curl_init();

    	curl_setopt_array($curlo, array(
    		CURLOPT_URL => "https://api.themoviedb.org/3/movie/192136/credits?api_key=bfeb66d29afeca7b6b4229bc7bd42938",
    		CURLOPT_RETURNTRANSFER => true,
    		CURLOPT_ENCODING => "",
    		CURLOPT_MAXREDIRS => 10,
    		CURLOPT_TIMEOUT => 30,
    		CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
    		CURLOPT_CUSTOMREQUEST => "GET",
    		CURLOPT_POSTFIELDS => "{}",
    	));

		$creditResponse = curl_exec($curlo);
		$creditErr = curl_error($curlo);

		curl_close($curlo);

		if ($creditErr) {
			echo "cURL Error #:" . $creditErr;
		} else {
			$creditDecodedResponse = json_decode($creditResponse, true);

			var_dump($creditDecodedResponse);
		}

		return NULL;
	}

    function getTestMethod ()
    {
    	$curl = curl_init();

    	curl_setopt_array($curl, array(
    		//CURLOPT_URL => "https://api.themoviedb.org/3/search/movie?include_adult=true&query=suicide%20squad&language=en-US&api_key=bfeb66d29afeca7b6b4229bc7bd42938",
    		CURLOPT_URL => "https://api.themoviedb.org/3/discover/movie?sort_by=popularity.desc&api_key=bfeb66d29afeca7b6b4229bc7bd42938",
    		//CURLOPT_URL => "https://api.themoviedb.org/3/movie/321612?api_key=bfeb66d29afeca7b6b4229bc7bd42938",
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

			//var_dump($results);

			//$testVar = array(
			//	'Title' => $decodedResponse['title'],
			//	'Description' => $decodedResponse['overview'],
			//	'Release' => $decodedResponse['release_date'],
			//	'Thumb' => $decodedResponse['poster_path'],
			//	'Homepage' => $decodedResponse['homepage'],
			//	'IMDB' => $decodedResponse['imdb_id'],
			//	'Runtime' => $decodedResponse['runtime'],
			//	'Language' => $decodedResponse['original_language'],
			//	'Status' => $decodedResponse['status']
			//);

			// Insert
			//insertDVD($testVar);

			//$testVar = array(
			//	'Title' => $results[0]['title'],
			//	'Description' => $results[0]['overview'],
			//	'Release' => $results[0]['release_date'],
			//	'Thumb' => $results[0]['poster_path'],
			//	'Homepage' => $results[0]['homepage'],
			//	'IMDB' => $results[0]['imdb_id'],
			//	'Runtime' => $results[0]['runtime'],
			//	'Language' => $results[0]['original_language'],
			//	'Status' => $results[0]['status']
			//);

			nabMovie($results[0]['id']);

			//var_dump($decodedResponse);

			//foreach ($results as $result)
			//{
				//$dvd = array ('Title' => $result['title'], 'Description' => $result['overview'], 'Release' => $result['release_date'], 'Thumb' => $result['poster_path']);
				//insertDVD($dvd);
			//}
		}

		return NULL;
    }

// EOF