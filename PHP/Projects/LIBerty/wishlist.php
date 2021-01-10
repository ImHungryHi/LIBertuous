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
	 * Values
	 * ----------------------------------------------------------------
	 */


	 	$txtSearch = isset($_POST['search']) ? (string) $_POST['search'] : '';
	 	$arrMovies = array();

	/**
	 * Only members allowed
	 * ----------------------------------------------------------------
	 */


		session_start();

		// Redirect to either login or wishlist
		if (!isset($_SESSION['userid']))
		{
			header('location: login.php');
		}
		elseif (!userInDb((int) $_SESSION['userid']))
		{
			$_SESSION['ErrorCode'] = 'missinguser';
			$_SESSION['ErrorMessage'] = 'Invalid user id: ' . $_SESSION['userid'];
			header('location: error.php');
		}

	/**
	 * Search code
	 * ----------------------------------------------------------------
	 */


	 	if (isset($_POST['formAction']) && $_POST['formAction'] == 'search')
	 	{
			if (trim($txtSearch) <> '')
			{
				// Filter that stuff
				$arrMovies = getFilteredWishlist((int) $_SESSION['userid'], $txtSearch);
			}
	 	}

	/**
	 * Main code
	 * ----------------------------------------------------------------
	 */


		// Assign the template stuff to variables
		$mainTpl = new Template('./core/layout/layout.tpl');
		$pageTpl = new Template('./templates/wishlist.tpl');

		$mainTpl->assign('pageTitle', 'LIBerty - wishlist');
		$mainTpl->assign('pageMeta', '<link rel="stylesheet" type="text/css" href="./core/styles/wishlist.css" />');
		$mainTpl->assignOption('oNavigation');

		// Start on the search form
		$pageTpl->assign('formUrl', $_SERVER['PHP_SELF']);

		if (sizeof($arrMovies < 1))
		{
			// Select ALL movies
			$arrMovies = getWishlisted((int) $_SESSION['userid']);
		}

		$arrMovies = getTestMethod();

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