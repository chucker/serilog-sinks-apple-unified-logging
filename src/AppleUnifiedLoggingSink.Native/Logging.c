//
//  Logging.c
//  Serilog.Sinks.AppleUnifiedLogging.Native
//
//  Created by Sören Kuklau on 08.03.20.
//  Copyright © 2020 Sören Kuklau. All rights reserved.
//

#include <os/log.h>

// See https://stackoverflow.com/questions/53711865/how-to-p-invoke-os-log/53795536#53795536

extern void LogDebug(os_log_t log, char *message) {
    os_log_debug(log, "%{public}s", message);
}

extern void LogInfo(os_log_t log, char *message) {
    os_log_info(log, "%{public}s", message);
}

extern void LogError(os_log_t log, char *message) {
    os_log_error(log, "%{public}s", message);
}

extern void LogFault(os_log_t log, char *message) {
    os_log_fault(log, "%{public}s", message);
}
