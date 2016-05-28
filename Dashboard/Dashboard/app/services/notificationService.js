(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .factory('notificationService', notificationService);

    function notificationService() {
        var internalNotificationList = [];

        var service = {
            addNotification: addNotification,
            addSuccess: addSuccess,
            addWarning: addWarning,
            addError: addError,
            removeLast: removeLast,
            removeAll: removeAll,
            init: init
        };

        return service;

        function addNotification(title, body, type) {
            if (internalNotificationList.length > 5) {
                internalNotificationList.shift();
            }
            internalNotificationList.push({ "title": title, "body": body, "type": type });
        }

        function addSuccess(title, body) {
            addNotification(title, body, 'success');
        }

        function addWarning(title, body) {
            addNotification(title, body, 'warn');
        }

        function addError(title, body) {
            addNotification(title, body, 'error');
        }

        function removeLast() {
            internalNotificationList.shift();
        }

        function removeAll() {
            //internalNotificationList.length = 0;
        }

        function init(notificationList) {
            if (angular.isArray(notificationList)) {
                internalNotificationList = notificationList;
            }
        }
    }
})();