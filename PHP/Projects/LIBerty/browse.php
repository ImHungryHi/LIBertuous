<?php

	/**
	 * Includes
	 * ----------------------------------------------------------------
	 */


		// config & functions
		require_once './core/includes/config.php';
		require_once './core/includes/functions.php';

		// needed classes
		require_once './core/includes/classes/template.php';
		require_once './core/includes/classes/database-1.2.php';

	/**
	 * Members only
	 * ----------------------------------------------------------------
	 */


		session_start();

		// Redirect to either login or wishlist
		if (!isset($_SESSION['userid']))
		{
			header('location: login.php');
		}

	/**
	 * Values
	 * ----------------------------------------------------------------
	 */


		$txtSearch = isset($_POST['search']) ? (string) $_POST['search'] : '';
	 	$arrMovies = array();

	/**
	 * Search code
	 * ----------------------------------------------------------------
	 */


	 	if (isset($_POST['formAction']) && $_POST['formAction'] == 'search')
	 	{
			if (trim($txtSearch) <> '')
			{
				// Filter that stuff
				$arrMovies = getFilteredMovies($txtSearch);
			}
	 	}

	/**
	 * Main code
	 * ----------------------------------------------------------------
	 */


		// Assign the template stuff to variables
		$mainTpl = new Template('./core/layout/layout.tpl');
		$pageTpl = new Template('./templates/browse.tpl');

		$mainTpl->assign('pageTitle', 'LIBerty - browse');
		$mainTpl->assign('pageMeta', '<link rel="stylesheet" type="text/css" href="./core/styles/browse.css" />');
		$mainTpl->assignOption('oNavigation');
		$mainTpl->assign('formUrl', $_SERVER['PHP_SELF']);

		if (sizeof($arrMovies < 1))
		{
			// Select ALL movies
			$arrMovies = getMovies((int) $_SESSION['userid']);
		}

		if (sizeof($arrMovies) === 0)
		{
			$pageTpl->assignOption('oNoMovies');
		}
		else
		{
			$pageTpl->assignOption('oMovies');
			$pageTpl->setIteration('iMovies');

			foreach ($arrMovies as $movie)
			{
				$pageTpl->assignIteration('iTitle', $movie['Title']);
				$pageTpl->assignIteration('iThumbPath', $movie['Thumb']);
				$pageTpl->assignIteration('iId', $movie['Id']);

				$arrDirectors = getDirectorsByMovieId($movie['Id']);
				$arrGenres = getGenresByMovieId($movie['Id']);

				$pageTpl->setIteration('iDirectors');

				foreach ($arrDirectors as $director)
				{
					$pageTpl->assignIteration('iDirector', $director['Name']);
					$pageTpl->assignIteration('iDirId', $director['Id']);
				}

				$pageTpl->parseIteration();

				$pageTpl->setIteration('iGenres');

				foreach ($arrGenres as $genre)
				{
					$pageTpl->assignIteration('iGenre', $genre['Title']);
					$pageTpl->assignIteration('iGenreId', $genre['Id']);
				}

				$pageTpl->parseIteration();
			}

			$pageTpl->parseIteration();
		}

		// Parse page specific content to the main page and show it
		$mainTpl->assign('pageContent', $pageTpl->getContent());
		$mainTpl->display();

// EOF