(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .factory('notificationService', notificationService);

    function notificationService() {
        var internalNotificationList = [];

        var service = {
            addNotification: addNotification,
            removeLast: removeLast,
            removeAll: removeAll,
            init: init
        };

        return service;

        function addNotification(title, body, type) {
            internalNotificationList.push({ "title": title, "body": body, "type": type });
        }

        function removeLast() {
            internalNotificationList.pop();
        }

        function removeAll() {
            internalNotificationList = [];
        }

        function init(notificationList) {
            if (angular.isArray(notificationList)) {
                internalNotificationList = notificationList;
            }
        }
    }
})();