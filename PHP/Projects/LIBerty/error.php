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
		$pageTpl = new Template('./templates/error.tpl');

		$mainTpl->assign('pageTitle', 'LIBerty - error');
		$mainTpl->assign('pageMeta', '<link rel="stylesheet" type="text/css" href="./core/styles/error.css" />');

		$txtError = 'You&lsquo;ve run into some magic error there. The grease monkeys are already on it!';

		// Check for details about the error
		if (isset($_SESSION['ErrorCode']))
		{
			switch (trim((string) $_SESSION['ErrorCode']))
			{
				case 'missinguser':
					$txtError = 'That&lsquo;s not supposed to happen. Who are you? Try <a href="/login.php">logging in</a> with a valid user.';

					if (isset($_SESSION['ErrorMessage']))
					{
						mail('chris.de.smedt.91@gmail.com', 'LIBerty error: ' . trim((string) $_SESSION['ErrorCode']), (string) $_SESSION['ErrorMessage']);
					}

					break;
				case '':
					$txtError = 'Oh, hey! Didn$lsquo;t see that one coming. I$lsquo;ll round up the fella$lsquo;s and get right on it!';

					if (isset($_SESSION['ErrorMessage']))
					{
						mail('chris.de.smedt.91@gmail.com', 'LIBerty error: ' . trim((string) $_SESSION['ErrorCode']), (string) $_SESSION['ErrorMessage']);
					}

					break;
				default:
					$txtError = 'Oh, hey! Didn$lsquo;t see that one coming. I$lsquo;ll round up the fella$lsquo;s and get right on it!';

					if (isset($_SESSION['ErrorMessage']))
					{
						mail('chris.de.smedt.91@gmail.com', 'LIBerty error (empty)', (string) $_SESSION['ErrorMessage']);
					}
			}
		}

		$pageTpl->assign('errorMessage', $txtError);

		// Parse page specific content to the main page and show it
		$mainTpl->assign('pageContent', $pageTpl->getContent());
		$mainTpl->display();


// EOF