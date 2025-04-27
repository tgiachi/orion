import { observer } from "mobx-react-lite";
import DefaultLayout from "../layouts/DefaultLayout";
import LoginForm from "../components/LoginForm";
import { useStore } from "../stores/rootStore";
import { useState } from "react";

export const LoginPage = observer(() => {

  const [errorMessage, setErrorMessage] = useState('')

  const rootStore = useStore()
  const loginHandle = async (username: string, password: string) => {

    rootStore.loadingStore.setLoading("")
    const result = await rootStore.authStore.login(username, password)

    if (result) {
      console.log('OK')
      setErrorMessage('')

    }
    else {
      console.log('Error')
      setErrorMessage("Wrong username / password!")
    }

    rootStore.loadingStore.hideLoading()

  }

  return <DefaultLayout>
    {!rootStore.authStore.isAuthicated && <LoginForm onLogin={loginHandle} isLoading={rootStore.loadingStore.isLoading} errorMessage={errorMessage} />}
  </DefaultLayout>
})
