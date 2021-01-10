<?php

	session_start();

	// Redirect to either login or wishlist
	if (isset($_SESSION['userid']))
	{
		header('location: wishlist.php');
	}
	else
	{
		header('location: login.php');
	}

// EOF