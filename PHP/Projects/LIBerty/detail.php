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


		//*todo*

	/**
	 * Specific code
	 * ----------------------------------------------------------------
	 */


		session_start();

	/**
	 * Main code
	 * ----------------------------------------------------------------
	 */


		// Redirect to either login or wishlist
		if (!isset($_SESSION['userid']))
		{
			header('location: login.php');
		}

		// Assign the template stuff to variables
		$mainTpl = new Template('./core/layout/layout.tpl');
		$pageTpl = new Template('./templates/*todo*.tpl');

		$mainTpl->assign('pageTitle', 'LIBerty - *todo*');
		$mainTpl->assign('pageMeta', '<link rel="stylesheet" type="text/css" href="./core/styles/*todo*.css" />');

		// Parse page specific content to the main page and show it
		$mainTpl->assign('pageContent', $pageTpl->getContent());
		$mainTpl->display();

// EOF