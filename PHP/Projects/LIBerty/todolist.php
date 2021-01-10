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

	/**
	 * Main code
	 * ----------------------------------------------------------------
	 */

		session_start();

		// Assign the template stuff to variables
		$mainTpl = new Template('./core/layout/layout.tpl');
		$pageTpl = new Template('./templates/todolist.tpl');

		$mainTpl->assign('pageTitle', 'LIBerty - todolist');
		$mainTpl->assign('pageMeta', '<link rel="stylesheet" type="text/css" href="./core/styles/todolist.css" />');

		// Parse page specific content to the main page and show it
		$mainTpl->assign('pageContent', $pageTpl->getContent());
		$mainTpl->display();


// EOF