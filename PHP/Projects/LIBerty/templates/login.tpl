					{option:oErrors}
					<div id="boxError">
						<p>One or more errors occured whilst processing your action:</p>
						<ul class="errors">
							{iteration:iErrors}
							<li>{$error|htmlentities}</li>
							{/iteration:iErrors}
						</ul>	
					</div>
					{/option:oErrors}
					<form action="{$formUrl|htmlentities}" method="post">
						<fieldset>
							<dl class="clearfix">
								<dd class="textbox"><input type="text" name="username" id="txtUsername" value="{$username|htmlentities}" placeholder="Username" /></dd>
								<dd class="textbox"><input type="password" name="password" id="txtPassword" placeholder="Password" /></dd>
								<dd id="buttons">
									<label for="btnSubmit"><input type="submit" id="btnSubmit" name="btnSubmit" value="Submit" /></label>
									<label for="btnRegister"><input type="submit" id="btnRegister" name="btnRegister" value="Register" /></label>
									<input type="hidden" name="formAction" id="formAction" value="login" />
								</dd>
							</dl>
						</fieldset>
					</form>