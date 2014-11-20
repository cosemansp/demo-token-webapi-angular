(function () {
    'use strict';

    angular.module('myApp')
        .controller("NavController", navController)
        .controller("MainController", mainController)
        .controller("LoginController", loginController)
        .controller("EditProfileController", editProfileController);

    function navController($log, $location, authService) {
        var vm = this;
        vm.logout = logout;

        activate();

        ///////

        function activate() {

        }

        function logout() {
            $log.info("logout");
            authService.logout().then(function () {
                $location.path("login");
            });
        }
    }

    function mainController() {
        var vm = this;
        vm.name = "main";

        activate();

        ///////

        function activate() {

        }
    }

    function editProfileController($http, $rootScope) {
        var vm = this;
        vm.apiKey = null;
        vm.apiKeyText = "";

        vm.name = "";
        vm.email = "";
        vm.addressLine = "";
        vm.city = "";
        vm.zip = "";
        vm.apiKeys = [];
        vm.deleteKey = deleteKey;
        vm.addKey = addKey;
        vm.userId = null;

        activate();

        ///////

        function activate() {
            vm.userId = $rootScope.identity.id;
            $http.get("api/users/" + vm.userId)
                .then(function(response) {
                    vm.name = response.data.name;
                    vm.email = response.data.email;
                    vm.addressLine = response.data.addressLine;
                    vm.city = response.data.city;
                    vm.zip = response.data.zip;
                });
            $http.get("api/users/" + vm.userId + "/apiKeys")
                .then(function (response) {
                    vm.apiKeys = response.data;
                });
        }

        function deleteKey(index) {
            var apiKey = vm.apiKeys[index];
            $http.delete("api/users/" + vm.userId + "/apiKeys/" + apiKey.name)
                .then(function (response) {
                    vm.apiKeys.splice(index, 1);
                });
        }

        function addKey() {
            $http.post("api/users/" + vm.userId + "/apiKeys", { name: vm.apiKey })
                .then(function(response) {
                    vm.apiKeyText = response.data.apiKey;
                    return $http.get("api/users/" + vm.userId + "/apiKeys");
                })
                .then(function (response) {
                    vm.apiKeys = response.data;
                });
        }
    }

    function loginController(authService, $location, $log) {
        var vm = this;
        vm.email = "peter.cosemans@euri.com";
        vm.password = "12345";
        vm.rememberMe = false;
        vm.login = login;
        vm.errorMessage = "";

        activate();

        ///////

        function activate() {

        }

        function login() {
            authService.authenticate(vm.email, vm.password, vm.rememberMe)
                .then(function (identity) {
                        $location.path("/main");
                    })
                    .catch(function(error) {
                        vm.errorMessage = "Failed to login, please try again.";
                    });
        }

        
    }

})();