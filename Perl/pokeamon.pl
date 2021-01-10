use Tk;
use strict;

my $mw = new MainWindow;
my $blnCheck = 1;

while ($blnCheck) {
	sleep 600;
	$mw->withdraw();
	my $result = $mw->messageBox(-message=>"Dag collega, kun je even een oogje werpen\nop eventuele kritische abends?\nControletoren dankt u!",-type=>"okcancel",-title=>"Poke a mon...itor");

	if ($result eq "Cancel") {
		$blnCheck = 0;
	}
}