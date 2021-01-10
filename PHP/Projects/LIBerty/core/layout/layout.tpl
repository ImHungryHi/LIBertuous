<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="nl">
<html>
	<head>
		<title>{$pageTitle}</title>
		<meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1" />
		<link rel="stylesheet" type="text/css" href="../core/styles/main.css" />
		
		{$pageMeta}
	</head>

	<body>
		<div id="sitewrapper">
			<header class="clearfix">
				<h1><span>LIB</span>erty</h1>				
				{option:oNavigation}
				<nav>
				    <ul>
					<li><a href="browse.php">Browse</a></li>
					<li><a href="wishlist.php">Wishlist</a></li>
					<li><a href="owned.php">Owned</a></li>
					<li><a href="friends.php">Friends</a></li>
				    </ul>
				    
				    <form action="{$formUrl|htmlentities}" method="post">
					<dl class="clearfix reset">
						<dd class="textbox"><input type="text" name="search" id="txtSearch" value="" placeholder="Search" /></dd>
						<dd class="hidden">
							<input type="hidden" name="formAction" id="formAction" value="search" />
						</dd>
					</dl>
				    </form>
				</nav>
				{/option:oNavigation}
			</header>
			
			<section id="mainwrapper">
				{$pageContent}
				
			</section>

			<footer>Made by someone who knows their stuff</footer>
		<div>
	</body>
</html>