import { observer } from "mobx-react-lite"
import { useStore } from './stores/rootStore'
import { Button } from "@heroui/button"
import DefaultLayout from "./layouts/DefaultLayout"


const App = observer(() => {

  const rootStore = useStore()



  return (
    <>
      <DefaultLayout>
        <h1>{rootStore.authStore.counter}</h1>
        <Button>Press me</Button>
      </DefaultLayout>
    </>
  )
})




export default App
