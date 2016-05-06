'use strict';
app.controller('tokensManagerController', ['$scope', 'tokensManagerService', 'notificationService',
    function ($scope, tokensManagerService, notificationService) {

        $scope.refreshTokens = [];
        $scope.clients = [];

        $scope.authClient = {
            id: undefined,
            name: undefined,
            applicationType: "0",
            allowedOrigin: undefined
        };

        $scope.secret = undefined;

        tokensManagerService.getRefreshTokens().then(function (results) {

            $scope.refreshTokens = results.data;

        }, function (error) {
            alert(error.data.message);
        });

        $scope.deleteRefreshTokens = function (index, tokenid) {

            tokenid = window.encodeURIComponent(tokenid);

            tokensManagerService.deleteRefreshTokens(tokenid).then(function (results) {
                $scope.refreshTokens.splice(index, 1);

            }, function (error) {
                alert(error.data.message);
            });
        }

        $scope.createApp = function () {
            tokensManagerService.createApp($scope.authClient)
                .then(function (response) {
                    var data = response.data.item1;
                    var secret = response.data.item2;
                    data.applicationType = data.applicationType + '';
                    $scope.authClient = data;
                    $scope.secret = secret;
                    $scope.getAllClients();
                },
                    function (error) {
                        notificationService.addNotification("create app", "action failed " + error, "error");
                    });
        };

        $scope.getAllClients = function () {
            tokensManagerService.getClients()
                .then(function (results) {
                    $scope.clients = results.data;
                },
                    function (error) {
                        notificationService.addNotification("clients list", "action failed " + error, "error");
                    });
        };

        $scope.newAppCreated = function () {
            return $scope.authClient.id;
        };

        $scope.getAllClients();

    }]);