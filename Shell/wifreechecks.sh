#!/bin/bash
let intCurTime=$(date +'%s')
if ping -c 3 8.8.8.8 &> /dev/null; then
        # Nothing to do here...
        # echo 1 >> /dev/null
        if [ -f /tmp/wifreebackuplockfornextrun.lck ]; then
		let intBackupTime=$(date +'%s' -r /tmp/wifreebackuplockfornextrun.lck)
		let intResult=$intCurTime-$intBackupTime

		if [ $intResult -gt 50 ]; then
			echo $(date -u) "Deleting backup lock because age > 50s" >> /var/log/wifreetimer.log
			rm /tmp/wifreebackuplockfornextrun.lck
		fi
	fi

	if [ -f /tmp/triplecheck.lck ]; then
		let intTripleTime=$(date +'%s' -r /tmp/triplecheck.lck)
		let intResult=$intCurTime-$intBackupTime

		if [ $intResult -gt 30 ]; then
			echo $(date -u) "Deleting second backup lock because age > 30s" >> /var/log/wifreetimer.log
			rm /tmp/triplecheck.lck
		fi
	fi
        # echo $(date -u) "It works" >> /var/log/wifreetimer.log
else
        if [ -f /tmp/wifreebackuplockfornextrun.lck ]; then
                let intBackupTime=$(date +'%s' -r /tmp/wifreebackuplockfornextrun.lck)
                let intResult=$intCurTime-$intBackupTime

                if [ $intResult > 50 ]; then
			echo $(date -u) "Deleting backup lock because age > 50s" >> /var/log/wifreetimer.log
                        rm /tmp/wifreebackuplockfornextrun.lck
                fi
        fi

        if [ -f /tmp/triplecheck.lck ]; then
                let intTripleTime=$(date + '%s' -r /tmp/triplecheck.lck)
                let intResult=$intCurTime-$intTripleTime
                rm /tmp/triplecheck.lck

                if [ $intResult < 30 ]; then
                        echo $(date -u) "Rebooting because it's all FUBAR" >> /var/log/wifreetimer.log
                        $(reboot)
		else
			echo $(date -u) "Deleting second backup lock because age > 30s" >> /var/log/wifreetimer.log
                fi
        else
		let intBackupTime=$(date +'%s' -r /tmp/wifreebackuplockfornextrun.lck)
		let intResult=$intCurTime-$intBackupTime

                if [ -f /tmp/wifreebackuplockfornextrun.lck ] && [ $intResult -gt 18 ]; then
			if [ ! -f /tmp/wifreelockfile.lck ]; then
	                        echo $(date -u) "Lock in doubt, waiting" >> /var/log/wifreetimer.log
	                        sleep 5
			fi

                        if [ -f /tmp/wifreebackuplockfornextrun.lck ]; then
				if [ ! -f /tmp/triplecheck.lck ]; then
	                                touch /tmp/triplecheck.lck
				fi

                                rm /tmp/wifreebackuplockfornextrun.lck
                                echo $(date -u) "Doubt validated, restarting networking service" >> /var/log/wifreetimer.log
                                $(systemctl restart networking)
                        fi
                else
                        if [ ! -f /tmp/wifreelockfile.lck ]; then
                                touch /tmp/wifreelockfile.lck
                                ip neigh flush dev wlan1
                                ifconfig wlan1 down
                                sleep 5
                                ifconfig wlan1 up
                                rm /tmp/wifreelockfile.lck
                                echo $(date -u) "Rebooted the wireless" >> /var/log/wifreetimer.log
                        fi

			if [ ! -f /tmp/wifreebackuplockfornextrun.lck ]; then
                                touch /tmp/wifreebackuplockfornextrun.lck
                        fi
                fi
	fi
fi
