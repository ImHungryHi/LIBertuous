<?php

	session_start();

	// Redirect to either login or wishlist
	if (isset($_SESSION['userid']))
	{
		unset($_SESSION['userid']);
	}

	header('location: login.php');

// EOF