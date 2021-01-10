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


	 	$formErrors = array();
	 	$username = isset($_POST['username']) ? $_POST['username'] : '';
	 	$password = isset($_POST['password']) ? $_POST['password'] : '';

	/**
	 * Login/Register code
	 * ----------------------------------------------------------------
	 */


		session_start();

	 	if (isset($_POST['formAction']) && $_POST['formAction'] == 'login')
	 	{
			if (trim($username) === '')
			{
				$formErrors[] = 'Please fill in a username';
			}
			else
			{
				if (trim($password) === '')
				{
					$formErrors[] = 'Please fill in a password';
				}
				else
				{
					if (isset($_POST['btnSubmit']))
					{
						$_SESSION['userid'] = login($username, $password);
						header('location: wishlist.php');
					}

					if (isset($_POST['btnRegister']))
					{
						$_SESSION['userid'] = register($username, $password);
						header('location: wishlist.php');
					}
				}
			}
	 	}

	/**
	 * Main code
	 * ----------------------------------------------------------------
	 */


		// Redirect to either login or wishlist
		if (isset($_SESSION['userid']))
		{
			header('location: wishlist.php');
		}

		// Assign the template stuff to variables
		$mainTpl = new Template('./core/layout/layout.tpl');
		$pageTpl = new Template('./templates/login.tpl');

		$mainTpl->assign('pageTitle', 'LIBerty - login');
		$mainTpl->assign('pageMeta', '<link rel="stylesheet" type="text/css" href="./core/styles/login.css" />');

		// Start filling the form
		$pageTpl->assign('formUrl', $_SERVER['PHP_SELF']);
		$pageTpl->assign('username', $username);

		// Error parsing
		if (sizeof($formErrors) > 0)
		{

			// assign the option
			$pageTpl->assignOption('oErrors');

			// set the iteration
			$pageTpl->setIteration('iErrors');

			// loop all items and store 'm into the template iteration
			foreach ($formErrors as $error)
			{
				// assign vars
				$pageTpl->assignIteration('error', 	$error);

				// refill iteration
				$pageTpl->refillIteration();
			}

			// parse iteration
			$pageTpl->parseIteration();
		}

		// Parse page specific content to the main page and show it
		$mainTpl->assign('pageContent', $pageTpl->getContent());
		$mainTpl->display();

// EOF